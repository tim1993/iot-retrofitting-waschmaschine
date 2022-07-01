
using System.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WashingIot.Configuration;
using System.Runtime.Caching;
using Timer = System.Timers.Timer;

namespace WashingIot.Data;

public class VelocityAggregationService : IHostedService
{
    private readonly Adx1345SensorDataCollector _sensorDataCollector;

    private readonly MemoryCache _cache = MemoryCache.Default;
    private readonly ILogger<VelocityAggregationService> _logger;
    private Timer? _timer;

    private VibrationMonitoringConfiguration _config;

    public delegate void AggregatedVelocityHistoryUpdatedHandler(IEnumerable<(DateTimeOffset, float)> values);

    public event AggregatedVelocityHistoryUpdatedHandler? AggregatedVelocityHistoryUpdated;

    public VelocityAggregationService(Adx1345SensorDataCollector sensorDataCollector, IOptionsSnapshot<VibrationMonitoringConfiguration> options, ILogger<VelocityAggregationService> logger)
    {
        _sensorDataCollector = sensorDataCollector;
        _logger = logger;
        _config = options.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {

        _timer = new Timer(TimeSpan.FromSeconds(15).TotalMilliseconds);
        _timer.Elapsed += OnElapsed;
        _timer.AutoReset = true;
        _timer.Enabled = true;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Stop();
        _timer?.Dispose();

        return Task.CompletedTask;
    }

    protected void OnElapsed(object? _, ElapsedEventArgs __)
    {
        CalculateSlidingAggregatedVelocity();
        var historicData = _cache.ToList().Select(data => ((DateTimeOffset, float))data.Value);
        AggregatedVelocityHistoryUpdated?.Invoke(historicData);
    }

    private void CalculateSlidingAggregatedVelocity()
    {
        _logger.LogInformation("Analyzing entries of last interval.");
        var readings = _sensorDataCollector.Readings.OrderBy(x => x.Timestamp).ToList();
        var velocity = readings.Zip(readings.Skip(1)).Select(x => new { Vx = x.First.Value.X - x.Second.Value.X, Vy = x.First.Value.Y - x.Second.Value.Y, Vz = x.First.Value.Z - x.Second.Value.Z });
        var combinedVelocity = velocity.Average(x => Math.Abs(x.Vx) + Math.Abs(x.Vy) + Math.Abs(x.Vz));

        _cache.Add(new CacheItem(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), (DateTimeOffset.Now, combinedVelocity)), new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now + TimeSpan.FromMinutes(30) });
    }
}