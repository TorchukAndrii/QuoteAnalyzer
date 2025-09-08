namespace QuoteAnalyzer.Statistics.ModeCounter;

public interface IModeCounter
{
    void Add(decimal value);
    decimal Mode();
}