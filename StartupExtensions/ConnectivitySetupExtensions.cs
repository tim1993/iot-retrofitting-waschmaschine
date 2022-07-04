using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WashingIot.Configuration;
using WashingIot.Connectivity;
using WashingIot.Data.Persistence;

namespace WashingIot.StartupExtensions;

public static class ConnectivitySetupExtensions
{
    public static void AddInfluxDbWriter(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptionsSnapshot<ConnectionConfiguration>>().Value;

            return InfluxDB.Client.InfluxDBClientFactory.Create(options.InfluxDbHost, options.InfluxDbToken);
        });

        services.AddSingleton<CsvPersistenceService>();
    }

    public static void AddAzIoTCentralTelemetry(this IServiceCollection services)
    {
        services.AddSingleton<DeviceClient>((sp) => DeviceClient.CreateFromConnectionString(sp.GetRequiredService<IOptionsSnapshot<ConnectionConfiguration>>().Value.IoTHubConnectionString));
        services.AddHostedService<ActivityTelemetryService>();
    }

}