using Microsoft.Extensions.Logging;
using WashingIot.Data.Persistence;

namespace WashingIot.Data;

public class AsyncSensorDataCollector<T>
{
    public IEnumerable<Reading<T>> Readings => _data.ToList();

    private readonly TimeSpan _samplingInterval;
    private readonly ISensorDataSource<T> _source;

    private readonly CancellationTokenSource _cts = new();
    private readonly TimeSpan _maxAge;
    private readonly ILogger? _logger;
    private Task? _collectorTask;

    private List<Reading<T>> _data = new();
    public AsyncSensorDataCollector(TimeSpan samplingInterval, TimeSpan maxAge, ISensorDataSource<T> source, ILogger? logger)
    {
        _samplingInterval = samplingInterval;
        _source = source;
        _maxAge = maxAge;
        _logger = logger;
    }

    public void Start() => _collectorTask = RunInternalAsync(_cts.Token);

    public void Stop()
    {
        _cts.Cancel();
        _collectorTask?.Wait(3000);
        _collectorTask?.Dispose();
    }

    protected virtual void LogReading(T value)
    { }

    private Task RunInternalAsync(CancellationToken cancellationToken)
    {
        if (_collectorTask?.Status == TaskStatus.Running)
        {
            throw new InvalidOperationException("Cannot start twice.");
        }

        return Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var measurement = await _source.GetMeasurmentAsync();
                    LogReading(measurement);
                    _data.Add(new Reading<T>(DateTime.UtcNow, measurement));
                }
                catch (Exception e)
                {
                    _logger?.LogCritical(e, "Error occured:");
                }

                HandleCleanUp();

                await Task.Delay((int)_samplingInterval.TotalMilliseconds);
            }
        });
    }

    private IEnumerable<Reading<T>> GetEntriesForCleanup() => _data.Where(x => DateTime.UtcNow - x.Timestamp > _maxAge);

    private void HandleCleanUp()
    {
        var cleanupCandidates = GetEntriesForCleanup();
        if (cleanupCandidates.Any())
        {
            _logger?.LogInformation("Cleaning {count} from SensorDataCollector", cleanupCandidates.Count());

            foreach (var candidate in cleanupCandidates)
            {
                _data.Remove(candidate);
            }
        }
    }
}

public record Reading<T>(DateTimeOffset Timestamp, T Value);