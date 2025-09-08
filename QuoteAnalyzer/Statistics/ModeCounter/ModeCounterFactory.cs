namespace QuoteAnalyzer.Statistics.ModeCounter;

public static class ModeCounterFactory
{
    public static IModeCounter Create(ModeAlgorithm algo)
    {
        return algo switch
        {
            ModeAlgorithm.Dictionary => new DictionaryModeCounter(),
            ModeAlgorithm.SpaceSaving => new SpaceSavingModeCounter(1000),
            _ => throw new ArgumentOutOfRangeException(nameof(algo), algo, "Unknown mode algorithm")
        };
    }
}