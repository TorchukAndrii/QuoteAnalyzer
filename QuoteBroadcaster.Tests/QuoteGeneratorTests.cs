namespace QuoteBroadcaster.Tests;

public class QuoteGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnValueWithinRange()
    {
        var generator = new QuoteGenerator();
        decimal min = 10;
        decimal max = 20;
        decimal tick = 2;

        for (int i = 0; i < 100; i++)
        {
            decimal result = generator.Generate(min, max, tick);
            Assert.InRange(result, min, max);
            Assert.True((result - min) % tick == 0);
        }
    }
}