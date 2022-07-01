using CsvHelper.Configuration;

namespace WashingIot.Data.Persistence;

public record AggregatedVelocityRecord(DateTimeOffset Ts, double VarX, double VarY, double VarZ, double AggregatedVar);

public class AggregatedVelocityRecordMap : ClassMap<AggregatedVelocityRecord>
{
    public AggregatedVelocityRecordMap()
    {
        Map(m => m.Ts).Index(0).Name("ts");
        Map(m => m.VarX).Index(1).Name("varX");
        Map(m => m.VarY).Index(2).Name("varY");
        Map(m => m.VarZ).Index(3).Name("varZ");
        Map(m => m.AggregatedVar).Index(4).Name("aggregatedVar");
    }
}