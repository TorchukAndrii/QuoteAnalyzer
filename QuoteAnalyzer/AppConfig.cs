using System.Xml.Linq;

namespace QuoteAnalyzer;

public sealed class AppConfig
{
    public AppConfig(string path)
    {
        var config = XElement.Load(path);
        MulticastIP = config.Element("MulticastIP")?.Value ?? "239.0.0.222";
        Port = int.Parse(config.Element("Port")?.Value ?? "5000");
        ModeAlgorithm = config.Element("ModeAlgorithm")?.Value ?? "Dictionary";
    }

    public string MulticastIP { get; }
    public int Port { get; }
    public string ModeAlgorithm { get; }
}