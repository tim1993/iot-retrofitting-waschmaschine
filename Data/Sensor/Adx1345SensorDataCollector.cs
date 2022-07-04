using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WashingIot.Configuration;
using WashingIot.Data.Persistence;

namespace WashingIot.Data;

public class Adx1345SensorDataCollector : AsyncSensorDataCollector<Adxl345Reading>
{
    private readonly PersistenceService _persistenceService;

    public Adx1345SensorDataCollector(ISensorDataSource<Adxl345Reading> source, PersistenceService persistenceService, IOptionsSnapshot<VibrationMonitoringConfiguration> options, ILogger<Adx1345SensorDataCollector> logger)
        : base(AnalysisConstants.SamplingInterval, AnalysisConstants.ObservationPeriod, source, logger)
    {
        _persistenceService = persistenceService;
    }

    protected override void LogReading(Adxl345Reading measurement)
    {
        _persistenceService.Write(new AccelerationRecord(DateTimeOffset.UtcNow, measurement.X, measurement.Y, measurement.Z));
    }
}