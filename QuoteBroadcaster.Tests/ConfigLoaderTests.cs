using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using QuoteBroadcaster;

namespace QuoteBroadcaster.Tests
{
    public class ConfigLoaderTests
    {
        [Fact]
        public void LoadFromXElement_ShouldLoadCorrectValues()
        {
            var xml = new XElement("Config",
                new XElement("MulticastIP", "239.1.1.1"),
                new XElement("Port", "6000"),
                new XElement("MinValue", "5.5"),
                new XElement("MaxValue", "100.5"),
                new XElement("TickSize", "0.5")
            );

            var loader = new ConfigLoader();
            var config = loader.LoadFromXElement(xml);

            Assert.Equal("239.1.1.1", config.MulticastIP);
            Assert.Equal(6000, config.Port);
            Assert.Equal(5.5m, config.MinValue);
            Assert.Equal(100.5m, config.MaxValue);
            Assert.Equal(0.5m, config.TickSize);
        }

        [Fact]
        public void LoadFromXElement_ShouldUseDefaults_WhenElementsMissing()
        {
            var xml = new XElement("Config"); // empty config
            var loader = new ConfigLoader();
            var config = loader.LoadFromXElement(xml);

            Assert.Equal("239.0.0.222", config.MulticastIP);
            Assert.Equal(5000, config.Port);
            Assert.Equal(10.00m, config.MinValue);
            Assert.Equal(2000.00m, config.MaxValue);
            Assert.Equal(0.10m, config.TickSize);
        }
    }
}
