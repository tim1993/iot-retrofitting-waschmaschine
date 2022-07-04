using System.Device.Spi;
using System.Runtime.InteropServices;
using Iot.Device.Adxl345;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WashingIot.Configuration;
using WashingIot.Connectivity;
using WashingIot.Data;
using WashingIot.Data.Persistence;
using WashingIot.Services;
using WashingIot.StartupExtensions;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, HostBuilderContext context)
    {
        services.AddAdxl345Sensor();
        services.AddVibrationAnalysis();
        services.AddAzIoTCentralTelemetry();
        services.AddInfluxDbWriter();


        services.Configure<VibrationMonitoringConfiguration>(context.Configuration.GetSection(nameof(VibrationMonitoringConfiguration)));
        services.Configure<ConnectionConfiguration>(context.Configuration.GetSection(nameof(ConnectionConfiguration)));
    }
}