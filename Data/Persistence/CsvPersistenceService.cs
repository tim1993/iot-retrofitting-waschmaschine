using System.Globalization;
using CsvHelper;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Options;
using WashingIot.Configuration;

namespace WashingIot.Data.Persistence;

public class CsvPersistenceService
{
    private object _lock = new object();

    private InfluxDBClient _influxClient;
    private readonly IOptionsSnapshot<ConnectionConfiguration> _influxSettings;

    public CsvPersistenceService(IOptionsSnapshot<ConnectionConfiguration> influxSettings, InfluxDBClient influxClient)
    {
        _influxSettings = influxSettings;
        _influxClient = influxClient;
    }

    public void Write(AccelerationRecord record)
    {

        var measurement = PointData.Measurement("vibration")
                            .Field("X", record.X).Field("Y", record.Y)
                            .Field("Z", record.Z).Timestamp(record.Ts, WritePrecision.Ms);

        WriteInternal(measurement);
    }


    public void Write(AggregatedVelocityRecord record)
    {
        var measurement = PointData.Measurement("velocity_variance").Field("Xvar", record.VarX).Field("Yvar", record.VarY)
                            .Field("Zvar", record.VarZ).Field("aggregatedVar", record.AggregatedVar)
                            .Timestamp(record.Ts, WritePrecision.Ms);

        WriteInternal(measurement);
    }

    public void Write(VelocityData record)
    {
        var measurement = PointData.Measurement("velocity").Field("vX", record.vX)
                    .Field("vY", record.vY).Field("vZ", record.vZ).Timestamp(record.Ts, WritePrecision.Ms);

        WriteInternal(measurement);
    }


    private void WriteInternal(PointData p)
    {
        using var influxWriteApi = _influxClient.GetWriteApi();
        influxWriteApi.WritePoint(p,
                _influxSettings.Value.InfluxDbBucket, _influxSettings.Value.InfluxDbOrg);
    }
}
