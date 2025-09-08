using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace QuoteBroadcaster;

internal class Program
{
    private static readonly Random _random = new();
    private static readonly Encoding _encoding = Encoding.UTF8;

    public static async Task Main(string[] args)
    {
        var config = LoadConfig("config.xml");

        Console.WriteLine(
            $"Config: IP={config.MulticastIP}, Port={config.Port}, Range=[{config.MinValue};{config.MaxValue}], TickSize={config.TickSize}"
        );

        using var udpClient = new UdpClient();
        var remoteEP = new IPEndPoint(IPAddress.Parse(config.MulticastIP), config.Port);

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        Console.WriteLine("Streaming quotes... Press Ctrl+C to stop.");

        try
        {
            
            long seq = 0;

            while (!cts.Token.IsCancellationRequested)
            {
                seq++;
                decimal quote = GenerateRandomQuote(config.MinValue, config.MaxValue, config.TickSize);

                // Include sequence number in message: "seq:quote"
                string msg = $"{seq}:{quote:F2}";
                byte[] buffer = _encoding.GetBytes(msg);

                await udpClient.SendAsync(buffer, buffer.Length, remoteEP);
            }
        }
        catch (OperationCanceledException)
        {
            //Exit
        }
    }

    private static decimal GenerateRandomQuote(decimal min, decimal max, decimal tickSize)
    {
        int ticks = (int)((max - min) / tickSize);
        int tickIndex = _random.Next(0, ticks + 1);
        return min + tickIndex * tickSize;
    }

    private static BroadcasterConfig LoadConfig(string path)
    {
        var config = XElement.Load(path);
        return new BroadcasterConfig
        {
            MulticastIP = config.Element("MulticastIP")?.Value ?? "239.0.0.222",
            Port        = int.Parse(config.Element("Port")?.Value ?? "5000"),
            MinValue    = decimal.Parse(config.Element("MinValue")?.Value ?? "10.00"),
            MaxValue    = decimal.Parse(config.Element("MaxValue")?.Value ?? "2000.00"),
            TickSize    = decimal.Parse(config.Element("TickSize")?.Value ?? "0.10")
        };
    }

    private record BroadcasterConfig
    {
        public string MulticastIP { get; init; } = "239.0.0.222";
        public int Port { get; init; } = 5000;
        public decimal MinValue { get; init; } = 10.00m;
        public decimal MaxValue { get; init; } = 2000.00m;
        public decimal TickSize { get; init; } = 0.10m;
    }
}

