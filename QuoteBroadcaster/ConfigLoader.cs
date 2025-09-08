using System.Xml.Linq;

namespace QuoteBroadcaster;

internal class ConfigLoader
{
    public BroadcasterConfig LoadFromFile(string path)
    {
        var element = XElement.Load(path);
        return LoadFromXElement(element);
    }

    public BroadcasterConfig LoadFromXElement(XElement element)
    {
        return new BroadcasterConfig
        {
            MulticastIP = element.Element("MulticastIP")?.Value ?? "239.0.0.222",
            Port        = int.Parse(element.Element("Port")?.Value ?? "5000"),
            MinValue    = decimal.Parse(element.Element("MinValue")?.Value ?? "10.00"),
            MaxValue    = decimal.Parse(element.Element("MaxValue")?.Value ?? "2000.00"),
            TickSize    = decimal.Parse(element.Element("TickSize")?.Value ?? "0.10")
        };
    }
}