namespace QuoteAnalyzer.Statistics;

internal class ModeCalculator
{
    private readonly Dictionary<decimal, int> _counts = new();

    /// <summary>
    ///     Gets the current mode (most frequent value).
    ///     Returns 0 if no values were added.
    /// </summary>
    public decimal Mode
    {
        get
        {
            if (_counts.Count == 0)
                return 0m;

            return _counts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
    }

    /// <summary>
    ///     Adds a value to the frequency counter.
    /// </summary>
    public void Add(decimal value)
    {
        _counts[value] = _counts.GetValueOrDefault(value, 0) + 1;
    }
}