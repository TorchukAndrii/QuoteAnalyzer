using System.Net.Sockets;
using System.Text;

namespace QuoteBroadcaster.Tests;

public static class UdpTestHelper
{
    public static async Task<string> ReceiveUdpMessage(UdpClient listener, int timeoutMs = 500)
    {
        var task = listener.ReceiveAsync();
        if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
        {
            var result = task.Result;
            return Encoding.UTF8.GetString(result.Buffer);
        }
        throw new TimeoutException("No message received in time.");
    }
}