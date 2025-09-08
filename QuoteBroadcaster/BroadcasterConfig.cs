namespace QuoteBroadcaster;

internal  record BroadcasterConfig
{
    public string MulticastIP { get; init; } = "239.0.0.222";
    public int Port { get; init; } = 5000;
    public decimal MinValue { get; init; } = 10.00m;
    public decimal MaxValue { get; init; } = 2000.00m;
    public decimal TickSize { get; init; } = 0.10m;
}