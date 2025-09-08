using QuoteAnalyzer.Statistics;

namespace QuoteAnalyzer.Tests.Statistics;

public class WelfordTests
{
    [Fact]
    public void NewInstance_ShouldHaveZeroCountMeanAndStdDev()
    {
        var w = new Welford();

        Assert.Equal(0, w.Count);
        Assert.Equal(0m, w.Mean);
        Assert.Equal(0m, w.StdDev);
    }

    [Fact]
    public void PushSingleValue_ShouldUpdateCountAndMeanAndStdDev()
    {
        var w = new Welford();
        w.Push(10m);

        Assert.Equal(1, w.Count);
        Assert.Equal(10m, w.Mean);
        Assert.Equal(0m, w.StdDev); // std dev is zero with single value
    }

    [Fact]
    public void PushMultipleValues_ShouldComputeMeanCorrectly()
    {
        var w = new Welford();
        decimal[] values = { 1m, 2m, 3m, 4m, 5m };

        foreach (var v in values)
            w.Push(v);

        Assert.Equal(values.Length, w.Count);
        Assert.Equal(3m, w.Mean); // mean = (1+2+3+4+5)/5 = 3
    }

    [Fact]
    public void PushMultipleValues_ShouldComputeStdDevCorrectly()
    {
        var w = new Welford();
        decimal[] values = { 1m, 2m, 3m, 4m, 5m };

        foreach (var v in values)
            w.Push(v);

        // StdDev = sqrt( sum((x - mean)^2)/N )
        double expected = Math.Sqrt(((1-3)*(1-3) + (2-3)*(2-3) + (3-3)*(3-3) + (4-3)*(4-3) + (5-3)*(5-3)) / 5.0);
        Assert.Equal((decimal)expected, w.StdDev, 6); // precision up to 6 decimals
    }

    [Fact]
    public void PushNegativeAndPositiveValues_ShouldComputeCorrectly()
    {
        var w = new Welford();
        decimal[] values = { -10m, 0m, 10m };

        foreach (var v in values)
            w.Push(v);

        Assert.Equal(3, w.Count);
        Assert.Equal(0m, w.Mean);
        double expectedStdDev = Math.Sqrt(((-10-0)*(-10-0) + (0-0)*(0-0) + (10-0)*(10-0))/3.0);
        Assert.Equal((decimal)expectedStdDev, w.StdDev, 6);
    }
}