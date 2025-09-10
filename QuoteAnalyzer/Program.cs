using System.Threading.Channels;
using QuoteAnalyzer.Statistics;

namespace QuoteAnalyzer;

internal class Program
{
    private static void Main(string[] args)
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.xml");
        var config = new AppConfig(configPath);

        Console.WriteLine($"Listening on {config.MulticastIP}:{config.Port} ...");

        Console.WriteLine("Press Enter at any time to display current statistics...");
        Console.WriteLine("Press 'q' to quit.");

        var channel = Channel.CreateUnbounded<decimal>();
        var statisticsAggregator = new StatisticsAggregator();
        var receiver = new QuoteReceiver(config.MulticastIP, config.Port, channel.Writer);

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("Stopping...");
            e.Cancel = true;
            cts.Cancel();
        };

        // Receiver thread
        var receiverThread = new Thread(async () =>
        {
            try
            {
                await receiver.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // safe exit
            }
        })
        {
            IsBackground = true
        };

        // Consumer thread
        var consumerThread = new Thread(async () =>
        {
            try
            {
                await foreach (var value in channel.Reader.ReadAllAsync(cts.Token))
                    statisticsAggregator.Add(value);
            }
            catch (OperationCanceledException)
            {
                // safe exit
            }
        })
        {
            IsBackground = true
        };

        receiverThread.Start();
        consumerThread.Start();

        // UI loop
        while (!cts.IsCancellationRequested)
        {
            var input = Console.ReadLine();
            if (input?.Trim().Equals("q", StringComparison.OrdinalIgnoreCase) == true)
            {
                cts.Cancel();
                break;
            }

            Console.WriteLine("=== Current Report ===");
            Console.WriteLine(receiver.GetErrorsReport());
            Console.WriteLine(statisticsAggregator.GetStatisticsReport());
            Console.WriteLine("=====================");
        }

        // Wait for threads to finish
        receiverThread.Join();
        consumerThread.Join();
    }
}