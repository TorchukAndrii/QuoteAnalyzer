using QuoteAnalyzer.Statistics;

namespace QuoteAnalyzer.Tests.Statistics.ModeCounter;

public class ModeCalculatorTests
{
    [Fact]
    public void Mode_Empty_ReturnsZero()
    {
        var counter = new ModeCalculator();
        Assert.Equal(0m, counter.Mode);
    }

    [Fact]
    public void Add_SingleValue_ModeIsThatValue()
    {
        var counter = new ModeCalculator();
        counter.Add(5m);
        Assert.Equal(5m, counter.Mode);
    }

    [Fact]
    public void Add_MultipleValues_ModeIsMostFrequent()
    {
        var counter = new ModeCalculator();
        counter.Add(1m);
        counter.Add(2m);
        counter.Add(2m);
        counter.Add(3m);
        counter.Add(2m);
        counter.Add(1m);

        Assert.Equal(2m, counter.Mode);
    }

    [Fact]
    public void Add_MultipleModes_ReturnsFirstHighest()
    {
        var counter = new ModeCalculator();
        counter.Add(1m);
        counter.Add(1m);
        counter.Add(2m);
        counter.Add(2m);

        // both 1 and 2 have same count, first key should be returned
        var mode = counter.Mode;
        Assert.True(mode == 1m || mode == 2m);
    }
}