using System.Xml.Linq;
using QuoteAnalyzer.Statistics.ModeCounter;

namespace QuoteAnalyzer;

public sealed class AppConfig
{
    public AppConfig(string path)
    {
        var config = XElement.Load(path);
        MulticastIP = config.Element("MulticastIP")?.Value ?? "239.0.0.222";
        Port = int.Parse(config.Element("Port")?.Value ?? "5000");
        Mode = ParseModeAlgorithm(config.Element("ModeAlgorithm")?.Value);
    }

    public string MulticastIP { get; }
    public int Port { get; }
    public ModeAlgorithm Mode { get; }

    private static ModeAlgorithm ParseModeAlgorithm(string? modeText)
    {
        if (string.IsNullOrWhiteSpace(modeText))
            return ModeAlgorithm.Dictionary;

        switch (modeText.Trim().ToLowerInvariant())
        {
            case "dictionary": return ModeAlgorithm.Dictionary;
            case "spacesaving": return ModeAlgorithm.SpaceSaving;
            default: return ModeAlgorithm.Dictionary;
        }
    }
}