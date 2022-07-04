using System.Text;
using System.Text.Json;
using System.Timers;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WashingIot.Data;

namespace WashingIot.Connectivity;

public class ActivityTelemetryService : IHostedService
{
    private readonly DeviceClient _deviceClient;
    private readonly ActivityDetectionService _activityDetectionService;
    private readonly ILogger<ActivityTelemetryService> _logger;
    private bool _activityDetected = false;
    public ActivityTelemetryService(DeviceClient deviceClient, ActivityDetectionService activityDetectionService, ILogger<ActivityTelemetryService> logger)
    {
        _deviceClient = deviceClient;
        _activityDetectionService = activityDetectionService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _activityDetectionService.ActivityStateUpdated += HandleActivityStateUpdated;

        try
        {
            await SendStatus(_activityDetected);
        }
        catch { }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _activityDetectionService.ActivityStateUpdated -= HandleActivityStateUpdated;
        return Task.CompletedTask;
    }

    private async void HandleActivityStateUpdated(bool activityDetected)
    {
        if (this._activityDetected != activityDetected)
        {
            await SendStatus(activityDetected);
            this._activityDetected = activityDetected;
        }
    }

    private async Task SendStatus(bool status)
    {
        try
        {
            _logger.LogInformation("Sending new activity status: {status}", status);
            var message = new Message(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new ActivityTelemetryMessage(status),
                                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })));

            await _deviceClient.SendEventAsync(message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not send status to cloud.");
        }

    }


    private record ActivityTelemetryMessage(bool Active);
}