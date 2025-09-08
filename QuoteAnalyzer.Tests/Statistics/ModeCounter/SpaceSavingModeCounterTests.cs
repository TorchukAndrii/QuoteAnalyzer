using QuoteAnalyzer.Statistics.ModeCounter;

namespace QuoteAnalyzer.Tests.Statistics.ModeCounter;

public class SpaceSavingModeCounterTests
{
    [Fact]
    public void Mode_Empty_ReturnsZero()
    {
        var counter = new SpaceSavingModeCounter(3);
        Assert.Equal(0m, counter.Mode());
    }

    [Fact]
    public void Add_FewerThanKValues_ModeIsMostFrequent()
    {
        var counter = new SpaceSavingModeCounter(3);
        counter.Add(1m);
        counter.Add(2m);
        counter.Add(2m);
        Assert.Equal(2m, counter.Mode());
    }

    [Fact]
    public void Add_MoreThanKValues_ReplacesLeastFrequent()
    {
        var counter = new SpaceSavingModeCounter(2);
        counter.Add(1m);
        counter.Add(2m);
        counter.Add(3m); // triggers replacement

        // Mode must be either 2 or 3 depending on how replacement occurred
        var mode = counter.Mode();
        Assert.True(mode == 2m || mode == 3m);
    }

    [Fact]
    public void Add_RepeatedValues_MaintainsMode()
    {
        var counter = new SpaceSavingModeCounter(3);
        counter.Add(1m);
        counter.Add(1m);
        counter.Add(2m);
        counter.Add(2m);
        counter.Add(2m);
        counter.Add(3m);
        counter.Add(3m);
        counter.Add(3m);

        var mode = counter.Mode();
        Assert.True(mode == 2m || mode == 3m);
    }
}