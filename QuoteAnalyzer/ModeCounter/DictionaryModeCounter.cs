using QuoteAnalyzer.Statistics.ModeCounter;

namespace QuoteAnalyzer.ModeCounter;

internal class DictionaryModeCounter : IModeCounter
{
    private readonly Dictionary<decimal, int> _counts = new();

    public void Add(decimal value)
    {
        _counts[value] = _counts.GetValueOrDefault(value, 0) + 1;
    }

    public decimal Mode()
    {
        if (_counts.Count == 0) return 0m;
        return _counts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }
}