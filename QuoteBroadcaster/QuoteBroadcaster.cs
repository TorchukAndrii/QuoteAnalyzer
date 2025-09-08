using System.Net;
using System.Text;
using QuoteBroadcaster.UdpClientWrapper;

namespace QuoteBroadcaster;

internal class QuoteBroadcaster
{
    private readonly IUdpClientWrapper _udpClient;
    private readonly IPEndPoint _remoteEP;
    private long _seq = 0;

    public QuoteBroadcaster(IUdpClientWrapper udpClient, IPEndPoint remoteEP)
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