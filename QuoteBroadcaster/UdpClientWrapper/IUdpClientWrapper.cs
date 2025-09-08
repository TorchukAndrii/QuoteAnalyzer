using System.Net;

namespace QuoteBroadcaster.UdpClientWrapper;

public interface IUdpClientWrapper
{
    Task SendAsync(byte[] datagram, int bytes, IPEndPoint endPoint);
}