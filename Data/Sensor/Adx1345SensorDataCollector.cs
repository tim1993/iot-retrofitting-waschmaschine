using Microsoft.Extensions.Logging;

namespace WashingIot.Data;

public class Adx1345SensorDataCollector : AsyncSensorDataCollector<Adxl345Reading>
{
    public Adx1345SensorDataCollector(ISensorDataSource<Adxl345Reading> source, ILogger<Adx1345SensorDataCollector> logger) : base(TimeSpan.FromMilliseconds(200), source, logger)
    {
    }
}