using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WashingIot.Configuration;

namespace WashingIot.Data;

public class Adx1345SensorDataCollector : AsyncSensorDataCollector<Adxl345Reading>
{
    public Adx1345SensorDataCollector(ISensorDataSource<Adxl345Reading> source, IOptionsSnapshot<VibrationMonitoringConfiguration> options, ILogger<Adx1345SensorDataCollector> logger) 
        : base(TimeSpan.FromMilliseconds(options.Value.SensorSamplingIntervalMs), source, logger)
    {
    }
}