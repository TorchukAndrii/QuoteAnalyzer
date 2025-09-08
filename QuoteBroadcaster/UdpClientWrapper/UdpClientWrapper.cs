using System.Net;
using System.Net.Sockets;

namespace QuoteBroadcaster.UdpClientWrapper;

public class UdpClientWrapper : IUdpClientWrapper
{
    private readonly UdpClient _udpClient;

    public UdpClientWrapper(UdpClient udpClient)
    {
        _udpClient = udpClient;
    }

    public Task SendAsync(byte[] datagram, int bytes, IPEndPoint endPoint)
    {
        return _udpClient.SendAsync(datagram, bytes, endPoint);
    }
}