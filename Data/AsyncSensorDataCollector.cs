namespace WashingIot.Data;

class AsyncSensorDataCollector<T>
{
    private readonly TimeSpan _samplingInterval;
    private readonly ISensorDataSource<T> _source;

    private readonly CancellationTokenSource _cts = new();
    private readonly int _maxItemCount;
    private Task? _collectorTask;

    private List<(DateTimeOffset, T)> _data = new();
    public AsyncSensorDataCollector(TimeSpan samplingInterval, ISensorDataSource<T> source, int maxItemCount = 16144)
    {
        _samplingInterval = samplingInterval;
        _source = source;
        _maxItemCount = maxItemCount;
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

        return Task.Run(async () => {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var measurement = await _source.GetMeasurmentAsync();
                    Console.WriteLine(measurement);
                    _data.Add((DateTime.UtcNow, measurement));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                if (NeedsCleanup())
                {
                    _data.Clear();
                }

                await Task.Delay((int)_samplingInterval.TotalMilliseconds);
            }
        });
    }

    private bool NeedsCleanup() => _data.Count >= _maxItemCount;
}