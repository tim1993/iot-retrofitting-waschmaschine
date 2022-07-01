using CsvHelper.Configuration;

namespace WashingIot.Data.Persistence;

public record ActivityDetectionRecord(DateTimeOffset Ts, float comparisonPeriodVelocity, float detectionPeriodVelocity, float procentualDifference);

public class ActivityDetectionRecordMap : ClassMap<ActivityDetectionRecord>
{
    public ActivityDetectionRecordMap()
    {
        Map(m => m.Ts).Index(0).Name("ts");
        Map(m => m.comparisonPeriodVelocity).Index(1).Name("vComparison");
        Map(m => m.detectionPeriodVelocity).Index(2).Name("vDetection");
        Map(m => m.procentualDifference).Index(3).Name("change");

    }
}