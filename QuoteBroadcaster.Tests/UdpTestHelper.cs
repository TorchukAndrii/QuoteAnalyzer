using System.Net.Sockets;

public static class UdpTestHelper
{
    public static async Task<byte[]> ReceiveUdpBytes(UdpClient listener, int timeoutMs = 500)
    {
        var receiveTask = listener.ReceiveAsync();
        var delayTask = Task.Delay(timeoutMs);

        var completed = await Task.WhenAny(receiveTask, delayTask);
        if (completed == receiveTask)
        {
            return receiveTask.Result.Buffer;
        }

        throw new TimeoutException("No UDP message received within the timeout.");
    }
}