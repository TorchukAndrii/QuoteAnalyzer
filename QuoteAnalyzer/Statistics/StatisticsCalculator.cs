using QuoteAnalyzer.Statistics.ModeCounter;

namespace QuoteAnalyzer.Statistics;

public sealed class StatisticsCalculator
{
    private readonly object _statsLock = new();
    
    private readonly IModeCounter _modeCounter;
    private readonly P2QuantileEstimator _p2Median = new(0.5m);
    private readonly Welford _stats = new();

    public StatisticsCalculator(IModeCounter modeCounter)
    {
        _modeCounter = modeCounter;
    }

    public void Add(decimal value)
    {
        lock (_statsLock)
        {
            _stats.Push(value);
            _p2Median.Add(value);
            _modeCounter.Add(value);
        }
    }

    public string GetReport(long received, long parseErrors, long networkErrors)
    {
        lock (_statsLock)
        {
            return $"Received: {received}, ParseErrors: {parseErrors}, NetworkErrors: {networkErrors}\n" +
                   $"Total processed: {_stats.Count}, " +
                   $"Arithmetic mean: {_stats.Mean}, " +
                   $"Standard deviation: {_stats.StdDev}, " +
                   $"Mode: {_modeCounter.Mode()}, " +
                   $"Median: {_p2Median.Estimate}";
        }
    }
}