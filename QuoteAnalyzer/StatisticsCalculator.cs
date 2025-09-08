using QuoteAnalyzer.ModeCounter;

namespace QuoteAnalyzer;

public sealed class StatisticsCalculator
{
    private readonly IModeCounter _modeCounter;
    private readonly P2QuantileEstimator _p2Median = new(0.5);
    private readonly Welford _stats = new();

    public StatisticsCalculator(IModeCounter modeCounter)
    {
        _modeCounter = modeCounter;
    }

    public void Add(double value)
    {
        _stats.Push(value);
        _p2Median.Add(value);
        _modeCounter.Add(value);
    }

    public string GetReport(long received, long parseErrors, long networkErrors)
    {
        return
            $"Received: {received}, ParseErrors: {parseErrors}, NetworkErrors: {networkErrors}\n" +
            $"Total processed: {_stats.Count}, " +
            $"Arithmetic mean: {_stats.Mean}, " +
            $"Standard deviation: {_stats.StdDev}, " +
            $"Mode: {_modeCounter.Mode()}, " +
            $"Median: {_p2Median.Estimate}";
    }
}