using CsvHelper.Configuration;

namespace WashingIot.Data.Persistence;

public record AggregatedVelocityRecord(DateTimeOffset Ts, float Vx, float Vy, float Vz, float AggregatedV);

public class AggregatedVelocityRecordMap : ClassMap<AggregatedVelocityRecord>
{
    public AggregatedVelocityRecordMap()
    {
        Map(m => m.Ts).Index(0).Name("ts");
        Map(m => m.Vx).Index(1).Name("vx");
        Map(m => m.Vy).Index(2).Name("vy");
        Map(m => m.Vz).Index(3).Name("vz");
        Map(m => m.AggregatedV).Index(4).Name("aggregatedV");
    }
}