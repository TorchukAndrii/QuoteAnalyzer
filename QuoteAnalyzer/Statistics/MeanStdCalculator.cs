namespace QuoteAnalyzer.Statistics;

/// <summary>
///     Calculator for mean and standard deviation using Welford’s algorithm.
/// </summary>
public sealed class MeanStdCalculator
{
    private long _count;
    private decimal _mean;
    private decimal _sumOfSquares; // M2 in Welford’s algorithm
    public decimal Mean => _count > 0 ? _mean : 0m;
    public decimal StandardDeviation => _count > 0 ? (decimal)Math.Sqrt((double)(_sumOfSquares / _count)) : 0m;

    /// <summary>
    ///     Adds a new sample value and updates mean + standard deviation.
    /// </summary>
    public void Add(decimal value)
    {
        _count++;
        var delta = value - _mean;
        _mean += delta / _count;
        _sumOfSquares += delta * (value - _mean);
    }
}