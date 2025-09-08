using System.Net;
using System.Net.Sockets;
using System.Text;
using Moq;
using QuoteBroadcaster.UdpClientWrapper;

namespace QuoteBroadcaster.Tests;

public class QuoteBroadcasterTests
{
    [Fact]
    public async Task BroadcastQuoteAsync_ShouldSendCorrectMessage()
    {
        var udpMock = new Mock<IUdpClientWrapper>();
        IPEndPoint ep = new(IPAddress.Parse("239.0.0.222"), 5000);
        var broadcaster = new QuoteBroadcaster(udpMock.Object, ep);

        await broadcaster.BroadcastQuoteAsync(123.45m);

        udpMock.Verify(u => u.SendAsync(
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "1:123.45"),
            It.IsAny<int>(),
            ep
        ), Times.Once);
    }
    

    [Fact]
    public async Task BroadcastQuoteAsync_ShouldIncrementSequence()
    {
        var udpMock = new Mock<IUdpClientWrapper>();
        IPEndPoint ep = new(IPAddress.Parse("239.0.0.222"), 5000);
        var broadcaster = new QuoteBroadcaster(udpMock.Object, ep);

        await broadcaster.BroadcastQuoteAsync(1m);
        await broadcaster.BroadcastQuoteAsync(2m);

        udpMock.Verify(u => u.SendAsync(
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "1:1.00"),
            It.IsAny<int>(),
            ep
        ), Times.Once);

        udpMock.Verify(u => u.SendAsync(
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "2:2.00"),
            It.IsAny<int>(),
            ep
        ), Times.Once);
    }
}