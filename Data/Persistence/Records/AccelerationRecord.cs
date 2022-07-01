using CsvHelper.Configuration;

namespace WashingIot.Data.Persistence;

public record AccelerationRecord(DateTimeOffset Ts, float X, float Y, float Z);

public class AccelerationRecordMap : ClassMap<AccelerationRecord>
{
    public AccelerationRecordMap()
    {
        Map(m => m.Ts).Index(0).Name("ts");
        Map(m => m.X).Index(1).Name("x");
        Map(m => m.Y).Index(2).Name("y");
        Map(m => m.Z).Index(3).Name("z");
    }
}