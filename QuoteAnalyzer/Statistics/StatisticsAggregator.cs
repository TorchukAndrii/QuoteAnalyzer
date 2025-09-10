namespace QuoteAnalyzer.Statistics;

/// <summary>
///     Aggregates streaming statistics: mean, stddev, median, and mode.
///     Thread-safe.
/// </summary>
public sealed class StatisticsAggregator
{
    private readonly MeanStdCalculator _meanStdCalculator = new();
    private readonly MedianCalculator _medianCalculator = new();

    private readonly ModeCalculator _modeCalculator = new();
    private readonly object _statsLock = new();

    /// <summary>
    ///     Records a new observation and updates all statistics.
    /// </summary>
    public void Add(decimal value)
    {
        lock (_statsLock)
        {
            _meanStdCalculator.Add(value);
            _medianCalculator.Add(value);
            _modeCalculator.Add(value);
        }
    }

    /// <summary>
    ///     Returns a formatted statistics report.
    /// </summary>
    public string GetStatisticsReport()
    {
        lock (_statsLock)
        {
            return
                $"Total processed: {_meanStdCalculator.Count}, " +
                $"Arithmetic mean: {_meanStdCalculator.Mean}, " +
                $"Standard deviation: {_meanStdCalculator.StandardDeviation}, " +
                $"Mode: {_modeCalculator.Mode}, " +
                $"Median: {_medianCalculator.Median}";
        }
    }
}