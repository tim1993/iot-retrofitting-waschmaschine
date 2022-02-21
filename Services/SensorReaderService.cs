using Microsoft.Extensions.Hosting;
using WashingIot.Data;

namespace WashingIot.Services;

public class SensorReaderService : IHostedService
{
    private readonly Adx1345SensorDataCollector _collector;

    public SensorReaderService(Adx1345SensorDataCollector collector)
    {
        _collector = collector;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _collector.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _collector.Stop();

        return Task.CompletedTask;
    }
}
