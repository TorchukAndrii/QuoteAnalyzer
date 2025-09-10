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
        
        byte[] buffer = new byte[sizeof(long) + sizeof(decimal)];
        
        BitConverter.TryWriteBytes(buffer.AsSpan(0, sizeof(long)), _seq);
        
        int[] bits = decimal.GetBits(quote); 
        for (int i = 0; i < 4; i++)
        {
            BitConverter.TryWriteBytes(buffer.AsSpan(sizeof(long) + i * 4, 4), bits[i]);
        }

        await _udpClient.SendAsync(buffer, buffer.Length, _remoteEP);
    }
}