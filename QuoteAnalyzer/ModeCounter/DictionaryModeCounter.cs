namespace QuoteAnalyzer.ModeCounter;

internal class DictionaryModeCounter : IModeCounter
{
    private readonly Dictionary<double, int> _counts = new();

    public void Add(double value)
    {
        _counts[value] = _counts.GetValueOrDefault(value, 0) + 1;
    }

    public double Mode()
    {
        if (_counts.Count == 0) return 0.0;
        return _counts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }
}