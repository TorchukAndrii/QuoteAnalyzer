using QuoteAnalyzer.Statistics;

namespace QuoteAnalyzer.Tests.Statistics;

public class MeanStdCalculatorTests
{
    [Fact]
    public void NewInstance_ShouldHaveZeroCountMeanAndStandardDeviation()
    {
        var w = new MeanStdCalculator();

        Assert.Equal(0m, w.Mean);
        Assert.Equal(0m, w.StandardDeviation);
    }

    [Fact]
    public void PushSingleValue_ShouldUpdateCountAndMeanAndStandardDeviation()
    {
        var w = new MeanStdCalculator();
        w.Add(10m);

        Assert.Equal(10m, w.Mean);
        Assert.Equal(0m, w.StandardDeviation); // std dev is zero with single value
    }

    [Fact]
    public void PushMultipleValues_ShouldComputeMeanCorrectly()
    {
        var w = new MeanStdCalculator();
        decimal[] values = { 1m, 2m, 3m, 4m, 5m };

        foreach (var v in values)
            w.Add(v);

        Assert.Equal(3m, w.Mean); // mean = (1+2+3+4+5)/5 = 3
    }

    [Fact]
    public void PushMultipleValues_ShouldComputeStandardDeviationCorrectly()
    {
        var w = new MeanStdCalculator();
        decimal[] values = { 1m, 2m, 3m, 4m, 5m };

        foreach (var v in values)
            w.Add(v);

        // StandardDeviation = sqrt( sum((x - mean)^2)/N )
        double expected = Math.Sqrt(((1-3)*(1-3) + (2-3)*(2-3) + (3-3)*(3-3) + (4-3)*(4-3) + (5-3)*(5-3)) / 5.0);
        Assert.Equal((decimal)expected, w.StandardDeviation, 6); // precision up to 6 decimals
    }

    [Fact]
    public void PushNegativeAndPositiveValues_ShouldComputeCorrectly()
    {
        var w = new MeanStdCalculator();
        decimal[] values = { -10m, 0m, 10m };

        foreach (var v in values)
            w.Add(v);

        Assert.Equal(0m, w.Mean);
        double expectedStandardDeviation = Math.Sqrt(((-10-0)*(-10-0) + (0-0)*(0-0) + (10-0)*(10-0))/3.0);
        Assert.Equal((decimal)expectedStandardDeviation, w.StandardDeviation, 6);
    }
}