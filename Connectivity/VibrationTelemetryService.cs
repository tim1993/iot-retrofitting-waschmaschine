using System.Text;
using System.Text.Json;
using System.Timers;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Hosting;
using WashingIot.Data;
using Timer = System.Timers.Timer;

namespace WashingIot.Connectivity;

public class VibrationTelemetryService : IHostedService
{
    private readonly DeviceClient deviceClient;
    private readonly ActivityDetectionService activityDetectionService;

    public VibrationTelemetryService(DeviceClient deviceClient, ActivityDetectionService activityDetectionService)
    {
        this.deviceClient = deviceClient;
        this.activityDetectionService = activityDetectionService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        activityDetectionService.ActivityStateUpdated += HandleActivityStateUpdated;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        activityDetectionService.ActivityStateUpdated -= HandleActivityStateUpdated;
        return Task.CompletedTask;
    }

    private async void HandleActivityStateUpdated(bool activityDetected)
    {
        var message = new Message(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new ActivityTelemetryMessage(activityDetected),
                                 new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })));

        await deviceClient.SendEventAsync(message);
    }


    private record ActivityTelemetryMessage(bool Active);
}