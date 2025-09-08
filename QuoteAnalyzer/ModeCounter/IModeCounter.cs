namespace QuoteAnalyzer.ModeCounter;

public interface IModeCounter
{
    void Add(double value);
    double Mode();
}