using System.Globalization;
using CsvHelper;

namespace WashingIot.Data.Persistence;

public class CsvPersistenceService
{
    private CsvWriter _accelerationWriter;
    private CsvWriter _aggregatedVelocityWriter;
    public CsvPersistenceService()
    {
        _accelerationWriter = CreateAccelerationWriter();
        _aggregatedVelocityWriter = CreateAggregatedVelocityWriter();
    }

    public void Write(AccelerationRecord record)
    {
        _accelerationWriter.WriteRecord(record);
    }

    public void Write(AggregatedVelocityRecord record)
    {
        _aggregatedVelocityWriter.WriteRecord(record);
    }

    private CsvWriter CreateAggregatedVelocityWriter()
    {
        var writer = CreateWriter("velocity.csv");
        writer.Context.RegisterClassMap<AggregatedVelocityRecordMap>();
        writer.WriteHeader<AggregatedVelocityRecord>();

        return writer;
    }

    private CsvWriter CreateAccelerationWriter()
    {
        var writer = CreateWriter("acceleration.csv");
        writer.Context.RegisterClassMap<AccelerationRecordMap>();
        writer.WriteHeader<AccelerationRecord>();

        return writer;
    }

    private CsvWriter CreateWriter(string file)
    {
        var streamWriter = new StreamWriter(File.Open(file, FileMode.Create));
        var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        return csvWriter;
    }
}
