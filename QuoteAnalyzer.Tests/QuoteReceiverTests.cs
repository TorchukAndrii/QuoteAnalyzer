using System.Threading.Channels;
using System.Net;
using System.Net.Sockets;
using System.Text;
using QuoteAnalyzer;

public class QuoteReceiverTests
{
    private int GetRandomPort() => new Random().Next(20000, 40000);

    [Fact]
    public async Task ShouldParseAndWriteValues()
    {
        var channel = Channel.CreateUnbounded<decimal>();
        int port = GetRandomPort();
        string multicastIP = "239.0.0.222"; // valid local multicast

        var receiver = new QuoteReceiver(multicastIP, port, channel.Writer);

        var cts = new CancellationTokenSource();
        var task = receiver.RunAsync(cts.Token);

        // send messages
        await UdpTestHelper.SendUdpMessage("1:10.5", multicastIP, port);
        await UdpTestHelper.SendUdpMessage("2:20.5", multicastIP, port);

        await Task.Delay(100); // wait for receiver

        cts.Cancel();
        await task;

        var values = new List<decimal>();
        await foreach (var v in channel.Reader.ReadAllAsync())
            values.Add(v);

        Assert.Equal(new decimal[] { 10.5m, 20.5m }, values);
        Assert.Equal(2, receiver.ReceivedCount);
        Assert.Equal(0, receiver.ParseErrors);
        Assert.Equal(0, receiver.NetworkErrors);
        Assert.Equal(0, receiver.LostCount);
    }

    [Fact]
    public async Task ShouldCountParseErrors()
    {
        var channel = Channel.CreateUnbounded<decimal>();
        int port = GetRandomPort();
        string multicastIP = "239.0.0.222";

        var receiver = new QuoteReceiver(multicastIP, port, channel.Writer);
        var cts = new CancellationTokenSource();
        var task = receiver.RunAsync(cts.Token);

        await UdpTestHelper.SendUdpMessage("bad:data", multicastIP, port);
        await Task.Delay(50);

        cts.Cancel();
        await task;

        Assert.Equal(1, receiver.ReceivedCount);
        Assert.Equal(1, receiver.ParseErrors);
    }

    [Fact]
    public async Task ShouldCountLostMessages()
    {
        var channel = Channel.CreateUnbounded<decimal>();
        int port = GetRandomPort();
        string multicastIP = "239.0.0.222";

        var receiver = new QuoteReceiver(multicastIP, port, channel.Writer);
        var cts = new CancellationTokenSource();
        var task = receiver.RunAsync(cts.Token);

        await UdpTestHelper.SendUdpMessage("1:10", multicastIP, port);
        await UdpTestHelper.SendUdpMessage("3:20", multicastIP, port); // seq 2 missing
        await Task.Delay(50);

        cts.Cancel();
        await task;

        Assert.Equal(2, receiver.ReceivedCount);
        Assert.Equal(0, receiver.ParseErrors);
        Assert.Equal(1, receiver.LostCount);
    }
}

public static class UdpTestHelper
{
    public static async Task SendUdpMessage(string message, string multicastIP, int port)
    {
        using var client = new UdpClient();
        var ep = new IPEndPoint(IPAddress.Parse(multicastIP), port);
        byte[] data = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(data, data.Length, ep);
    }
}
