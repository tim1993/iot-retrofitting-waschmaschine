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
    private readonly DeviceClient deviceClient;
    private readonly ActivityDetectionService activityDetectionService;
    private readonly ILogger<ActivityTelemetryService> logger;
    private bool activityDetected = false;
    public ActivityTelemetryService(DeviceClient deviceClient, ActivityDetectionService activityDetectionService, ILogger<ActivityTelemetryService> logger)
    {
        this.deviceClient = deviceClient;
        this.activityDetectionService = activityDetectionService;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        activityDetectionService.ActivityStateUpdated += HandleActivityStateUpdated;

        try
        {
            await SendStatus(activityDetected);
        }
        catch { }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        activityDetectionService.ActivityStateUpdated -= HandleActivityStateUpdated;
        return Task.CompletedTask;
    }

    private async void HandleActivityStateUpdated(bool activityDetected)
    {
        if (this.activityDetected != activityDetected)
        {
            await SendStatus(activityDetected);
            this.activityDetected = activityDetected;
        }
    }

    private async Task SendStatus(bool status)
    {
        logger.LogInformation("Sending new activity status: {status}", activityDetected);
        var message = new Message(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new ActivityTelemetryMessage(status),
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })));

        await deviceClient.SendEventAsync(message);
    }


    private record ActivityTelemetryMessage(bool Active);
}