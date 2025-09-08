using System.Net;
using System.Net.Sockets;

namespace QuoteBroadcaster;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.xml");
        var configLoader = new ConfigLoader();
        var config = configLoader.LoadFromFile(configPath);

        Console.WriteLine(
            $"Config: IP={config.MulticastIP}, Port={config.Port}, Range=[{config.MinValue};{config.MaxValue}], TickSize={config.TickSize}"
        );

        Console.WriteLine("QuoteBroadcaster started. Press Ctrl+C to stop streaming quotes.");

        var udpClient = new UdpClient();
        var remoteEP = new IPEndPoint(IPAddress.Parse(config.MulticastIP), config.Port);

        var generator = new QuoteGenerator();
        var broadcaster = new QuoteBroadcaster(udpClient, remoteEP);

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        while (!cts.Token.IsCancellationRequested)
        {
            var quote = generator.Generate(config.MinValue, config.MaxValue, config.TickSize);
            await broadcaster.BroadcastQuoteAsync(quote);
        }
    }
}