using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace QuoteAnalyzer.Tests;

public class QuoteReceiverTests
{
    private int GetRandomPort() => new Random().Next(20000, 40000);

    [Fact]
    public async Task ShouldParseAndWriteValues_Binary()
    {
        var channel = Channel.CreateUnbounded<decimal>();
        int port = GetRandomPort();
        string multicastIP = "239.0.0.222";

        var receiver = new QuoteReceiver(multicastIP, port, channel.Writer);
        var cts = new CancellationTokenSource();
        var task = receiver.RunAsync(cts.Token);

        // send binary messages
        await UdpTestHelper.SendUdpMessage(1, 10.5m, multicastIP, port);
        await UdpTestHelper.SendUdpMessage(2, 20.5m, multicastIP, port);

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
    public async Task ShouldCountParseErrors_Binary()
    {
        var channel = Channel.CreateUnbounded<decimal>();
        int port = GetRandomPort();
        string multicastIP = "239.0.0.222";

        var receiver = new QuoteReceiver(multicastIP, port, channel.Writer);
        var cts = new CancellationTokenSource();
        var task = receiver.RunAsync(cts.Token);

        // send invalid message (wrong length)
        using (var client = new UdpClient())
        {
            var ep = new IPEndPoint(IPAddress.Parse(multicastIP), port);
            await client.SendAsync(new byte[] { 1, 2, 3 }, 3, ep);
        }

        await Task.Delay(50);

        cts.Cancel();
        await task;

        Assert.Equal(1, receiver.ReceivedCount);
        Assert.Equal(1, receiver.ParseErrors);
    }

    [Fact]
    public async Task ShouldCountLostMessages_Binary()
    {
        var channel = Channel.CreateUnbounded<decimal>();
        int port = GetRandomPort();
        string multicastIP = "239.0.0.222";

        var receiver = new QuoteReceiver(multicastIP, port, channel.Writer);
        var cts = new CancellationTokenSource();
        var task = receiver.RunAsync(cts.Token);

        // send messages skipping seq=2
        await UdpTestHelper.SendUdpMessage(1, 10m, multicastIP, port);
        await UdpTestHelper.SendUdpMessage(3, 20m, multicastIP, port);

        await Task.Delay(50);

        cts.Cancel();
        await task;

        Assert.Equal(2, receiver.ReceivedCount);
        Assert.Equal(0, receiver.ParseErrors);
        Assert.Equal(1, receiver.LostCount); // seq=2 missing
    }
}