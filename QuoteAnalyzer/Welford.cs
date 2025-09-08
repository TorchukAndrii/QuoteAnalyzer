namespace QuoteAnalyzer;

internal class Welford
{
    private double _m2;
    private double _mean;

    public double Mean => Count > 0 ? _mean : 0.0;

    public double StdDev => Count > 0 ? Math.Sqrt(_m2 / Count) : 0.0;

    public long Count { get; private set; }

    public void Push(double x)
    {
        Count++;
        var delta = x - _mean;
        _mean += delta / Count;
        _m2 += delta * (x - _mean);
    }
}