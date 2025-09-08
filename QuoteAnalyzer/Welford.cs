namespace QuoteAnalyzer;

internal class Welford
{
    private decimal _m2;
    private decimal _mean;

    public decimal Mean => Count > 0 ? _mean : 0m;

    public decimal StdDev => Count > 0 ? (decimal)Math.Sqrt((double)(_m2 / Count)) : 0m;

    public long Count { get; private set; }

    public void Push(decimal x)
    {
        Count++;
        var delta = x - _mean;
        _mean += delta / Count;
        _m2 += delta * (x - _mean);
    }
}