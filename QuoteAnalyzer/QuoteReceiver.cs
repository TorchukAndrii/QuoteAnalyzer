using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;

namespace QuoteAnalyzer;

public sealed class QuoteReceiver : IAsyncDisposable
{
    private readonly UdpClient _udpClient;
    private readonly ChannelWriter<double> _writer;
    
    private long _networkErrors;
    private long _parseErrors;
    private long _receivedCount;

    public QuoteReceiver(string multicastIP, int port, ChannelWriter<double> writer)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _udpClient = new UdpClient();
        var localEP = new IPEndPoint(IPAddress.Any, port);
        _udpClient.Client.Bind(localEP);
        _udpClient.JoinMulticastGroup(IPAddress.Parse(multicastIP));
    }

    public long ReceivedCount => _receivedCount;
    public long ParseErrors => _parseErrors;
    public long NetworkErrors => _networkErrors;


    public ValueTask DisposeAsync()
    {
        _udpClient.Dispose();
        return ValueTask.CompletedTask;
    }

    public async Task RunAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
                try
                {
                    var result = await _udpClient.ReceiveAsync(token);
                    Interlocked.Increment(ref _receivedCount);

                    var text = Encoding.UTF8.GetString(result.Buffer).Trim();
                    if (double.TryParse(text, out var value))
                        _writer.TryWrite(value);
                    else
                        Interlocked.Increment(ref _parseErrors);
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
        finally
        {
            _writer.TryComplete();
        }
    }
}