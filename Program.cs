using System.Device.Spi;
using Iot.Device.Adxl345;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WashingIot.Data;
using WashingIot.Services;

await CreateHostBuilder(args).Build().RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
                services.AddSingleton(new Adxl345(SpiDevice.Create(new SpiConnectionSettings(0, 0)
                {
                    ClockFrequency = Adxl345.SpiClockFrequency,
                    Mode = Adxl345.SpiMode
                }), GravityRange.Range08));

                services.AddSingleton<Adx1345SensorDataCollector>();
                services.AddSingleton<ISensorDataSource<Adxl345Reading>, Adxl345DataSource>(); 
                services.AddHostedService<SensorReaderService>();
        });