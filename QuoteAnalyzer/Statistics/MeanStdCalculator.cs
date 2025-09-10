namespace QuoteAnalyzer.Statistics;

/// <summary>
///     Online calculator for mean and standard deviation using Welford’s algorithm.
/// </summary>
public sealed class MeanStdCalculator
{
    private decimal _mean;
    private decimal _sumOfSquares; // M2 in Welford’s algorithm

    public long Count { get; private set; }
    public decimal Mean => Count > 0 ? _mean : 0m;
    public decimal StandardDeviation => Count > 0 ? (decimal)Math.Sqrt((double)(_sumOfSquares / Count)) : 0m;

    /// <summary>
    ///     Adds a new sample value and updates mean + standard deviation.
    /// </summary>
    public void Add(decimal value)
    {
        Count++;
        var delta = value - _mean;
        _mean += delta / Count;
        _sumOfSquares += delta * (value - _mean);
    }
}