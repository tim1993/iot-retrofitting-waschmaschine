using System.Timers;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Hosting;
using WashingIot.Data;
using Timer = System.Timers.Timer;

namespace WashingIot.Connectivity;

public class VibrationTelemetryService : IHostedService
{
    private readonly static TimeSpan Delay = TimeSpan.FromSeconds(30);
    private readonly VelocityAggregationService _analysisService;
    private Timer? _timer;
    public VibrationTelemetryService(DeviceClient client, VelocityAggregationService analysisService)
    {
        _analysisService = analysisService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        _timer = new Timer(Delay.TotalMilliseconds);
        _timer.Elapsed += OnTimerElapsedAsync;
        _timer.Enabled = true;

        return Task.CompletedTask;
    }

    private async Task OnElapsedAsync()
    {

    }

    private async void OnTimerElapsedAsync(object? _, ElapsedEventArgs __)
    {
        await OnElapsedAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}