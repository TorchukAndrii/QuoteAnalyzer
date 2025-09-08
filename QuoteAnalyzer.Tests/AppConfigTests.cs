using System.Xml.Linq;
using QuoteAnalyzer.Statistics.ModeCounter;

namespace QuoteAnalyzer.Tests;

public class AppConfigTests
{
    [Fact]
    public void ShouldLoadValuesFromXml()
    {
        var xml = new XElement("Config",
            new XElement("MulticastIP", "239.1.1.1"),
            new XElement("Port", "6000"),
            new XElement("ModeAlgorithm", "SpaceSaving")
        );

        string path = "test.xml";
        xml.Save(path);

        var config = new AppConfig(path);

        Assert.Equal("239.1.1.1", config.MulticastIP);
        Assert.Equal(6000, config.Port);
        Assert.Equal(ModeAlgorithm.SpaceSaving, config.Mode);
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
        Assert.Equal(ModeAlgorithm.Dictionary, config.Mode);
    }

    [Theory]
    [InlineData("dictionary", ModeAlgorithm.Dictionary)]
    [InlineData("spacesaving", ModeAlgorithm.SpaceSaving)]
    [InlineData("", ModeAlgorithm.Dictionary)]
    [InlineData(null, ModeAlgorithm.Dictionary)]
    [InlineData("unknown", ModeAlgorithm.Dictionary)]
    public void ParseModeAlgorithm_ShouldReturnExpectedMode(string input, ModeAlgorithm expected)
    {
        var result = typeof(AppConfig)
            .GetMethod("ParseModeAlgorithm", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, new object?[] { input });

        Assert.Equal(expected, result);
    }
}