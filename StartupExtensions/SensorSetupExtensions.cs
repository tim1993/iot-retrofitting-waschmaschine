using System.Device.Spi;
using System.Runtime.InteropServices;
using Iot.Device.Adxl345;
using Microsoft.Extensions.DependencyInjection;
using WashingIot.Connectivity;
using WashingIot.Data;
using WashingIot.Services;

namespace WashingIot.StartupExtensions;

public static class SensorSetupExtensions
{
    public static void AddAdxl345Sensor(this IServiceCollection services)
    {
        services.AddSingleton<Adx1345SensorDataCollector>();

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
            }), GravityRange.Range08));
            services.AddSingleton

            <ISensorDataSource<Adxl345Reading>, Adxl345DataSource>();
        }
    }

    public static void AddVibrationAnalysis(this IServiceCollection services)
    {
        services.AddSingleton<SensorReaderService>().AddHostedService(sp => sp.GetRequiredService<SensorReaderService>());
        services.AddSingleton<VelocityAggregationService>().AddHostedService(sp => sp.GetRequiredService<VelocityAggregationService>());
        services.AddSingleton<ActivityDetectionService>().AddHostedService(sp => sp.GetRequiredService<ActivityDetectionService>());
    }
}