using System.Net;
using System.Net.Sockets;

namespace QuoteBroadcaster.Tests;

public class QuoteBroadcasterTests
{
    private int GetRandomPort() => new Random().Next(20000, 40000);

    [Fact]
    public async Task ShouldBroadcastSingleQuote()
    {
        int port = GetRandomPort();
        using var listener = new UdpClient(port); // bind listener first
        var endpoint = new IPEndPoint(IPAddress.Loopback, port);
        var broadcaster = new QuoteBroadcaster(new UdpClient(), endpoint);

        await broadcaster.BroadcastQuoteAsync(1.00m);

        string message = await UdpTestHelper.ReceiveUdpMessage(listener);
        Assert.Equal("1:1.00", message);
    }

    [Fact]
    public async Task ShouldIncrementSequenceForMultipleQuotes()
    {
        int port = GetRandomPort();
        using var listener = new UdpClient(port);
        var endpoint = new IPEndPoint(IPAddress.Loopback, port);
        var broadcaster = new QuoteBroadcaster(new UdpClient(), endpoint);

        await broadcaster.BroadcastQuoteAsync(1.00m);
        await broadcaster.BroadcastQuoteAsync(2.50m);

        string msg1 = await UdpTestHelper.ReceiveUdpMessage(listener);
        string msg2 = await UdpTestHelper.ReceiveUdpMessage(listener);

        Assert.Equal("1:1.00", msg1);
        Assert.Equal("2:2.50", msg2);
    }

    [Fact]
    public async Task ShouldFormatDecimalWithTwoDigits()
    {
        int port = GetRandomPort();
        using var listener = new UdpClient(port);
        var endpoint = new IPEndPoint(IPAddress.Loopback, port);
        var broadcaster = new QuoteBroadcaster(new UdpClient(), endpoint);

        await broadcaster.BroadcastQuoteAsync(123.4567m);

        string msg = await UdpTestHelper.ReceiveUdpMessage(listener);
        Assert.Equal("1:123.46", msg);
    }
}