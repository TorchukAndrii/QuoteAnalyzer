using System.Net;
using System.Net.Sockets;
using QuoteBroadcaster.UdpClientWrapper;

namespace QuoteBroadcaster;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var configLoader = new ConfigLoader();
        var config = configLoader.LoadFromFile("config.xml");
        
        Console.WriteLine(
            $"Config: IP={config.MulticastIP}, Port={config.Port}, Range=[{config.MinValue};{config.MaxValue}], TickSize={config.TickSize}"
        );
        
        Console.WriteLine("QuoteBroadcaster started. Press Ctrl+C to stop streaming quotes.");
        
        var udpClient = new UdpClient();
        IUdpClientWrapper udpWrapper = new UdpClientWrapper.UdpClientWrapper(udpClient);
        var remoteEP = new IPEndPoint(IPAddress.Parse(config.MulticastIP), config.Port);

        var generator = new QuoteGenerator();
        var broadcaster = new QuoteBroadcaster(udpWrapper, remoteEP);

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        while (!cts.Token.IsCancellationRequested)
        {
            decimal quote = generator.Generate(config.MinValue, config.MaxValue, config.TickSize);
            await broadcaster.BroadcastQuoteAsync(quote);
        }
    }

    
    
}