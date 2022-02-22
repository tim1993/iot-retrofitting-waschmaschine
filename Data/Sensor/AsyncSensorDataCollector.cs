using Microsoft.Extensions.Logging;

namespace WashingIot.Data;

public class AsyncSensorDataCollector<T>
{
    public IEnumerable<Reading<T>> Readings => _data.ToList();

    private readonly TimeSpan _samplingInterval;
    private readonly ISensorDataSource<T> _source;

    private readonly CancellationTokenSource _cts = new();
    private readonly int _maxItemCount;
    private readonly ILogger? _logger;
    private Task? _collectorTask;

    private List<Reading<T>> _data = new();
    public AsyncSensorDataCollector(TimeSpan samplingInterval, ISensorDataSource<T> source, ILogger? logger, int maxItemCount = 16144)
    {
        _samplingInterval = samplingInterval;
        _source = source;
        _maxItemCount = maxItemCount;
        _logger = logger;
    }

    public void Start() => _collectorTask = RunInternalAsync(_cts.Token);

    public void Stop()
    {
        _cts.Cancel();
        _collectorTask?.Wait(3000);
        _collectorTask?.Dispose();
    }

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
                    Console.WriteLine(measurement);
                    _data.Add(new Reading<T>(DateTime.UtcNow, measurement));
                }
                catch (Exception e)
                {
                    _logger?.LogCritical(e, "Error occured:");
                }

                if (NeedsCleanup())
                {
                    _data.RemoveAt(1);
                }

                await Task.Delay((int)_samplingInterval.TotalMilliseconds);
            }
        });
    }

    private bool NeedsCleanup() => _data.Count >= _maxItemCount;
}

public record Reading<T>(DateTimeOffset Timestamp, T Value);