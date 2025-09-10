using System.Net;
using System.Net.Sockets;

namespace QuoteAnalyzer.Tests;

public static class UdpTestHelper
{
    public static async Task SendUdpMessage(long seq, decimal value, string multicastIP, int port)
    {
        using var client = new UdpClient();
        var ep = new IPEndPoint(IPAddress.Parse(multicastIP), port);

        // convert seq (long) to 8 bytes, decimal to 16 bytes
        byte[] buffer = new byte[24];
        BitConverter.TryWriteBytes(buffer.AsSpan(0, 8), seq);

        int[] bits = decimal.GetBits(value); // 4 ints = 16 bytes
        for (int i = 0; i < 4; i++)
            BitConverter.TryWriteBytes(buffer.AsSpan(8 + i * 4, 4), bits[i]);

        await client.SendAsync(buffer, buffer.Length, ep);
    }
}