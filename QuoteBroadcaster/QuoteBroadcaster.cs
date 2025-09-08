using System.Net;
using System.Net.Sockets;
using System.Text;

namespace QuoteBroadcaster;

internal class QuoteBroadcaster
{
    private readonly UdpClient _udpClient;
    private readonly IPEndPoint _remoteEP;
    private long _seq = 0;

    public QuoteBroadcaster(UdpClient udpClient, IPEndPoint remoteEP)
    {
        _udpClient = udpClient;
        _remoteEP = remoteEP;
    }

    public async Task BroadcastQuoteAsync(decimal quote)
    {
        _seq++;
        string msg = $"{_seq}:{quote:F2}";
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        await _udpClient.SendAsync(buffer, buffer.Length, _remoteEP);
    }
}