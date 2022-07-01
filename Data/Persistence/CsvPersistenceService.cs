using System.Globalization;
using CsvHelper;

namespace WashingIot.Data.Persistence;

public class CsvPersistenceService
{
    private CsvWriter _accelerationWriter;
    private CsvWriter _aggregatedVelocityWriter;
    private CsvWriter _activityWriter;

    public CsvPersistenceService()
    {
        _accelerationWriter = CreateAccelerationWriter();
        _aggregatedVelocityWriter = CreateAggregatedVelocityWriter();
        _activityWriter = CreateActivityDetectionWriter();
    }

    public void Write(AccelerationRecord record)
        => WriteInternal(_accelerationWriter, record);


    public void Write(AggregatedVelocityRecord record)
        => WriteInternal(_aggregatedVelocityWriter, record);

    public void Write(ActivityDetectionRecord record)
        => WriteInternal(_activityWriter, record);

    private void WriteInternal<T>(CsvWriter writer, T record)
    {
        writer.WriteRecord(record);
        writer.NextRecord();
        writer.Flush();
    }

    private CsvWriter CreateAccelerationWriter()
    {
        var writer = CreateWriter("acceleration.csv");
        writer.Context.RegisterClassMap<AccelerationRecordMap>();
        writer.WriteHeader<AccelerationRecord>();
        writer.NextRecord();

        return writer;
    }

    private CsvWriter CreateAggregatedVelocityWriter()
    {
        var writer = CreateWriter("velocity.csv");
        writer.Context.RegisterClassMap<AggregatedVelocityRecordMap>();
        writer.WriteHeader<AggregatedVelocityRecord>();
        writer.NextRecord();

        return writer;
    }

    private CsvWriter CreateActivityDetectionWriter()
    {
        var writer = CreateWriter("activity.csv");
        writer.Context.RegisterClassMap<ActivityDetectionRecordMap>();
        writer.WriteHeader<ActivityDetectionRecord>();
        writer.NextRecord();

        return writer;
    }

    private CsvWriter CreateWriter(string file)
    {
        var streamWriter = new StreamWriter(File.Open($"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{file}", FileMode.Create));
        var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        return csvWriter;
    }
}
