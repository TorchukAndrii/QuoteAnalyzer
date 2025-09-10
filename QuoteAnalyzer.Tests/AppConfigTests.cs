using System.Xml.Linq;

namespace QuoteAnalyzer.Tests;

public class AppConfigTests
{
    [Fact]
    public void ShouldLoadValuesFromXml()
    {
        var xml = new XElement("Config",
            new XElement("MulticastIP", "239.1.1.1"),
            new XElement("Port", "6000")
        );

        string path = "test.xml";
        xml.Save(path);

        var config = new AppConfig(path);

        Assert.Equal("239.1.1.1", config.MulticastIP);
        Assert.Equal(6000, config.Port);
    }

    [Fact]
    public void ShouldUseDefaults_WhenMissingElements()
    {
        var xml = new XElement("Config"); // empty
        string path = "empty.xml";
        xml.Save(path);

        var config = new AppConfig(path);

        Assert.Equal("239.0.0.222", config.MulticastIP);
        Assert.Equal(5000, config.Port);
    }
}