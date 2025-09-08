using System.Threading.Channels;
using QuoteAnalyzer.ModeCounter;

namespace QuoteAnalyzer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var config = new AppConfig("config.xml");

        Console.WriteLine($"Listening on {config.MulticastIP}:{config.Port} ...");
        Console.WriteLine($"Mode algorithm: {config.ModeAlgorithm}");
        if (config.ModeAlgorithm.Equals("SpaceSaving", StringComparison.OrdinalIgnoreCase))
            Console.WriteLine("Warning: Space-Saving is approximate and may slightly affect mode accuracy.");

        var channel = Channel.CreateUnbounded<double>();
        var modeCounter = ModeCounterFactory.Create(config.ModeAlgorithm);
        var stats = new StatisticsCalculator(modeCounter);
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
                await foreach (var value in channel.Reader.ReadAllAsync()) stats.Add(value);
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

            Console.WriteLine("=== Current Statistics ===");
            Console.WriteLine(stats.GetReport(receiver.ReceivedCount, receiver.ParseErrors, receiver.NetworkErrors));
            Console.WriteLine("==========================");
        }

        await Task.WhenAll(receiverTask, consumerTask);
    }
}