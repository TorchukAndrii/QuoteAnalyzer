using System.Threading.Channels;
using QuoteAnalyzer.Statistics;
using QuoteAnalyzer.Statistics.ModeCounter;

namespace QuoteAnalyzer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.xml");
        var config = new AppConfig(configPath);

        Console.WriteLine($"Listening on {config.MulticastIP}:{config.Port} ...");
        Console.WriteLine($"Mode algorithm: {config.Mode}");

        if (config.Mode == ModeAlgorithm.SpaceSaving)
            Console.WriteLine("Warning: Space-Saving is approximate and may slightly affect mode accuracy.");

        Console.WriteLine("Press Enter at any time to display current statistics...");

        var channel = Channel.CreateUnbounded<decimal>();
        var modeCounter = ModeCounterFactory.Create(config.Mode); // pass enum
        var statsCalculator = new StatisticsCalculator(modeCounter);
        var receiver = new QuoteReceiver(config.MulticastIP, config.Port, channel.Writer);

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("Stopping...");
            e.Cancel = true;
            cts.Cancel();
        };

        var receiverTask = receiver.RunAsync(cts.Token);
        var consumerTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var value in channel.Reader.ReadAllAsync()) statsCalculator.Add(value);
            }
            catch (OperationCanceledException)
            {
                // safe exit
            }
        });

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
            Console.WriteLine(statsCalculator.GetStatisticsReport());
            Console.WriteLine("=====================");
        }

        await Task.WhenAll(receiverTask, consumerTask);
    }
}