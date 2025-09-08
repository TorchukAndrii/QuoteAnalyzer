namespace QuoteAnalyzer.Statistics.ModeCounter;

internal class SpaceSavingModeCounter : IModeCounter
{
    private readonly int _k;
    private readonly Dictionary<decimal, Entry> _table;

    public SpaceSavingModeCounter(int k)
    {
        _k = k;
        _table = new Dictionary<decimal, Entry>();
    }

    public void Add(decimal value)
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

    public decimal Mode()
    {
        if (_table.Count == 0) return 0m;
        return _table.Values.OrderByDescending(e => e.Count).First().Item;
    }

    private class Entry
    {
        public int Count;
        public int Error;
        public decimal Item;
    }
}