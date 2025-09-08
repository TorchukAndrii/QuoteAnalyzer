namespace QuoteBroadcaster;

internal class QuoteGenerator
{
    private readonly Random _random = new();

    public decimal Generate(decimal min, decimal max, decimal tickSize)
    {
        int ticks = (int)((max - min) / tickSize);
        int tickIndex = _random.Next(0, ticks + 1);
        return min + tickIndex * tickSize;
    }
}