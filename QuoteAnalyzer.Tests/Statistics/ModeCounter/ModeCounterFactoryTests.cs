using QuoteAnalyzer.Statistics.ModeCounter;

namespace QuoteAnalyzer.Tests.Statistics.ModeCounter;

public class ModeCounterFactoryTests
{
    [Fact]
    public void Create_DictionaryAlgorithm_ReturnsDictionaryModeCounter()
    {
        var counter = ModeCounterFactory.Create(ModeAlgorithm.Dictionary);
        Assert.IsType<DictionaryModeCounter>(counter);
    }

    [Fact]
    public void Create_SpaceSavingAlgorithm_ReturnsSpaceSavingModeCounter()
    {
        var counter = ModeCounterFactory.Create(ModeAlgorithm.SpaceSaving);
        Assert.IsType<SpaceSavingModeCounter>(counter);
    }

    [Fact]
    public void Create_InvalidAlgorithm_Throws()
    {
        Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            ModeCounterFactory.Create((ModeAlgorithm)999));
    }
}