namespace WashingIot.Configuration;

public static class AnalysisConstants
{
    public static readonly TimeSpan SamplingInterval = TimeSpan.FromMilliseconds(100);
    public static readonly TimeSpan ObservationPeriod = TimeSpan.FromMinutes(10);

    public static readonly TimeSpan ActivityDetectionComparisionBase = ObservationPeriod * 0.8;
}