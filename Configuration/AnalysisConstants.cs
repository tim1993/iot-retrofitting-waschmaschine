namespace WashingIot.Configuration;

public static class AnalysisConstants
{
    public static readonly TimeSpan SamplingInterval = TimeSpan.FromMilliseconds(100);
    public static readonly TimeSpan ObservationPeriod = TimeSpan.FromMinutes(30);

    public static readonly TimeSpan VarianceAggregationInterval = TimeSpan.FromSeconds(5);
}