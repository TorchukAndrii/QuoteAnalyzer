using System.Net;
using System.Net.Sockets;

namespace QuoteBroadcaster.Tests;

public class QuoteBroadcasterTests
{
    private int GetRandomPort() => new Random().Next(20000, 40000);

    private decimal ReadDecimalFromBuffer(byte[] buffer, int offset = 8)
    {
        int[] bits = new int[4];
        for (int i = 0; i < 4; i++)
            bits[i] = BitConverter.ToInt32(buffer, 8 + i * 4);

        return  new decimal(bits);
    }

    private long ReadSeqFromBuffer(byte[] buffer)
    {
        return BitConverter.ToInt64(buffer, 0);
    }

    [Fact]
    public async Task ShouldBroadcastSingleQuote()
    {
        int port = GetRandomPort();
        using var listener = new UdpClient(port);
        var endpoint = new IPEndPoint(IPAddress.Loopback, port);
        var broadcaster = new QuoteBroadcaster(new UdpClient(), endpoint);

        decimal quote = 1.00m;
        await broadcaster.BroadcastQuoteAsync(quote);

        var result = await UdpTestHelper.ReceiveUdpBytes(listener);
        long seq = ReadSeqFromBuffer(result);
        decimal value = ReadDecimalFromBuffer(result);

        Assert.Equal(1, seq);
        Assert.Equal(quote, value);
    }

    [Fact]
    public async Task ShouldIncrementSequenceForMultipleQuotes()
    {
        int port = GetRandomPort();
        using var listener = new UdpClient(port);
        var endpoint = new IPEndPoint(IPAddress.Loopback, port);
        var broadcaster = new QuoteBroadcaster(new UdpClient(), endpoint);

        decimal q1 = 1.00m;
        decimal q2 = 2.50m;

        await broadcaster.BroadcastQuoteAsync(q1);
        await broadcaster.BroadcastQuoteAsync(q2);

        var r1 = await UdpTestHelper.ReceiveUdpBytes(listener);
        var r2 = await UdpTestHelper.ReceiveUdpBytes(listener);

        Assert.Equal(1, ReadSeqFromBuffer(r1));
        Assert.Equal(q1, ReadDecimalFromBuffer(r1));

        Assert.Equal(2, ReadSeqFromBuffer(r2));
        Assert.Equal(q2, ReadDecimalFromBuffer(r2));
    }

    [Fact]
    public async Task ShouldSendDecimalExactly()
    {
        int port = GetRandomPort();
        using var listener = new UdpClient(port);
        var endpoint = new IPEndPoint(IPAddress.Loopback, port);
        var broadcaster = new QuoteBroadcaster(new UdpClient(), endpoint);

        decimal quote = 123.4567m;
        await broadcaster.BroadcastQuoteAsync(quote);

        var result = await UdpTestHelper.ReceiveUdpBytes(listener);
        decimal received = ReadDecimalFromBuffer(result);

        Assert.Equal(quote, received);
    }
}
