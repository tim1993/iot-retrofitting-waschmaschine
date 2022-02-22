using Microsoft.Extensions.Logging;

namespace WashingIot.Data;

class Adxl345SimulationDataSource : ISensorDataSource<Adxl345Reading>
{
    private readonly ILogger<Adxl345SimulationDataSource> _logger;

    private readonly Random _rnd = new();
    public Adxl345SimulationDataSource(ILogger<Adxl345SimulationDataSource> logger)
    {
        _logger = logger;
    }
    public Task<Adxl345Reading> GetMeasurmentAsync() 
            => Task.FromResult(new Adxl345Reading(_rnd.NextSingle(), _rnd.NextSingle(), _rnd.NextSingle()));
}