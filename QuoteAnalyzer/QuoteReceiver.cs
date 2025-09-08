using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;

namespace QuoteAnalyzer;

public sealed class QuoteReceiver : IAsyncDisposable
{
    private readonly UdpClient _udpClient;
    private readonly ChannelWriter<decimal> _writer;

    public QuoteReceiver(string multicastIP, int port, ChannelWriter<decimal> writer)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _udpClient = new UdpClient();
        var localEP = new IPEndPoint(IPAddress.Any, port);
        _udpClient.Client.Bind(localEP);
        _udpClient.JoinMulticastGroup(IPAddress.Parse(multicastIP));
    }

    private long _receivedCount;
    private long _parseErrors;
    private long _networkErrors;
    private long _lostCount;

    public long ReceivedCount => Interlocked.Read(ref _receivedCount);
    public long ParseErrors => Interlocked.Read(ref _parseErrors);
    public long NetworkErrors => Interlocked.Read(ref _networkErrors);
    public long LostCount => Interlocked.Read(ref _lostCount);



    public ValueTask DisposeAsync()
    {
        _udpClient.Dispose();
        return ValueTask.CompletedTask;
    }

    public async Task RunAsync(CancellationToken token)
    {
        long lastSeq = 0;

        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync(token);
                    Interlocked.Increment(ref _receivedCount);

                    var text = Encoding.UTF8.GetString(result.Buffer).Trim();
                    var parts = text.Split(':');

                    if (parts.Length == 2 &&
                        long.TryParse(parts[0], out long seq) &&
                        decimal.TryParse(parts[1], out var value))
                    {
                        if (lastSeq > 0 && seq != lastSeq + 1)
                        {
                            Interlocked.Add(ref _lostCount, seq - (lastSeq + 1));
                        }

                        lastSeq = seq;
                        _writer.TryWrite(value);
                    }
                    else
                    {
                        Interlocked.Increment(ref _parseErrors);
                    }
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    break; // Ctrl+C
                }
                catch (ObjectDisposedException)
                {
                    break; // disposed
                }
                catch (SocketException)
                {
                    break; // socket closed on shutdown
                }
                catch
                {
                    Interlocked.Increment(ref _networkErrors);
                }
            }
        }
        finally
        {
            _writer.TryComplete();
        }
    }
    public string GetErrorsReport()
    {
        return
            $"Received: {ReceivedCount}, " +
            $"ParseErrors: {ParseErrors}, " +
            $"NetworkErrors: {NetworkErrors}, " +
            $"Lost: {LostCount}";
    }
}