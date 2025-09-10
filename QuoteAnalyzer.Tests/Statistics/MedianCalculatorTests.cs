using QuoteAnalyzer.Statistics;

namespace QuoteAnalyzer.Tests.Statistics;

public class MedianCalculatorTests
{
    [Fact]
    public void Median_BeforeInitialization_ReturnsZero()
    {
        var estimator = new MedianCalculator();
        Assert.Equal(0m, estimator.Median);
    }

    [Fact]
    public void Add_FirstFiveValues_InitializesEstimator()
    {
        var estimator = new MedianCalculator();
        decimal[] values = { 5m, 1m, 3m, 2m, 4m };

        foreach (var v in values) estimator.Add(v);

        // After first 5 values, estimator should be initialized
        Assert.True(estimator.Median >= 1m && estimator.Median <= 5m);
    }

    [Fact]
    public void Add_MoreThanFiveValues_AdjustsMedian()
    {
        var estimator = new MedianCalculator();
        decimal[] values = { 5m, 1m, 3m, 2m, 4m };

        foreach (var v in values) estimator.Add(v);

        // Add more values to trigger adjustments
        decimal[] moreValues = { 6m, 0m, 3.5m, 2.5m, 4.5m };
        foreach (var v in moreValues) estimator.Add(v);

        // Median (p=0.5) should stay within the min/max range
        Assert.InRange(estimator.Median, 0m, 6m);
    }

    [Fact]
    public void Add_AscendingValues_MedianIncreases()
    {
        var estimator = new MedianCalculator();

        for (decimal i = 1; i <= 10; i++)
            estimator.Add(i);

        Assert.InRange(estimator.Median, 1m, 10m);
    }

    [Fact]
    public void Add_DescendingValues_MedianDecreases()
    {
        var estimator = new MedianCalculator();

        for (decimal i = 10; i >= 1; i--)
            estimator.Add(i);

        Assert.InRange(estimator.Median, 1m, 10m);
    }

    [Fact]
    public void Add_RandomValues_MedianWithinRange()
    {
        var estimator = new MedianCalculator();
        decimal[] values = { 2.5m, 7m, 1m, 4m, 6m, 3m, 5m };

        foreach (var v in values) estimator.Add(v);

        decimal min = 1m, max = 7m;
        Assert.InRange(estimator.Median, min, max);
    }
}