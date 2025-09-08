using QuoteAnalyzer.Statistics.ModeCounter;

namespace QuoteAnalyzer.ModeCounter;

internal class SpaceSavingModeCounter : IModeCounter
{
    private readonly int _k;
    private readonly Dictionary<double, Entry> _table;

    public SpaceSavingModeCounter(int k)
    {
        _k = k;
        _table = new Dictionary<double, Entry>();
    }

    public void Add(double value)
    {
        if (_table.TryGetValue(value, out var entry))
        {
            entry.Count++;
        }
        else
        {
            if (_table.Count < _k)
            {
                _table[value] = new Entry { Item = value, Count = 1, Error = 0 };
            }
            else
            {
                var minEntry = _table.Values.OrderBy(e => e.Count).First();
                _table.Remove(minEntry.Item);

                _table[value] = new Entry
                {
                    Item = value,
                    Count = minEntry.Count + 1,
                    Error = minEntry.Count
                };
            }
        }
    }

    public double Mode()
    {
        if (_table.Count == 0) return 0.0;
        return _table.Values.OrderByDescending(e => e.Count).First().Item;
    }

    private class Entry
    {
        public int Count;
        public int Error;
        public double Item;
    }
}