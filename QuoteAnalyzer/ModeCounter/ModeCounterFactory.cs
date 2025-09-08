namespace QuoteAnalyzer.ModeCounter;

public sealed class ModeCounterFactory
{
    public static IModeCounter Create(string algo)
    {
        return algo.Equals("Dictionary", StringComparison.OrdinalIgnoreCase)
            ? new DictionaryModeCounter()
            : new SpaceSavingModeCounter(1000);
    }
}