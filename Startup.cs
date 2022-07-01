using System.Device.Spi;
using System.Runtime.InteropServices;
using Iot.Device.Adxl345;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WashingIot.Configuration;
using WashingIot.Data;
using WashingIot.Data.Persistence;
using WashingIot.Services;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, HostBuilderContext context)
    {
        services.AddSingleton<Adx1345SensorDataCollector>();
        services.AddSingleton<CsvPersistenceService>();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            services.AddSingleton<ISensorDataSource<Adxl345Reading>, Adxl345SimulationDataSource>();
        }
        else
        {
            services.AddSingleton(new Adxl345(SpiDevice.Create(new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = Adxl345.SpiClockFrequency,
                Mode = Adxl345.SpiMode
            }), GravityRange.Range02));
            services.AddSingleton<ISensorDataSource<Adxl345Reading>, Adxl345DataSource>();
        }
        services.AddSingleton<SensorReaderService>().AddHostedService(sp => sp.GetRequiredService<SensorReaderService>());
        services.AddSingleton<VelocityAggregationService>().AddHostedService(sp => sp.GetRequiredService<VelocityAggregationService>());
        services.AddSingleton<ActivityDetectionService>().AddHostedService(sp => sp.GetRequiredService<ActivityDetectionService>());

        services.AddSingleton<DeviceClient>((sp) => DeviceClient.CreateFromConnectionString(sp.GetRequiredService<IOptionsSnapshot<ConnectionConfiguration>>().Value.IoTHubConnectionString));

        services.Configure<VibrationMonitoringConfiguration>(context.Configuration.GetSection(nameof(VibrationMonitoringConfiguration)));
        services.Configure<ConnectionConfiguration>(context.Configuration.GetSection(nameof(ConnectionConfiguration)));
    }
}