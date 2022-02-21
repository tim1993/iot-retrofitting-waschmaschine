// See https://aka.ms/new-console-template for more information
using System.Device.Spi;
using System.Numerics;
using Iot.Device.Adxl345;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WashingIot.Data;

SpiConnectionSettings settings = new SpiConnectionSettings(0, 0)
{
    ClockFrequency = Adxl345.SpiClockFrequency,
    Mode = Adxl345.SpiMode
};

var device = SpiDevice.Create(settings);

using (Adxl345 sensor = new Adxl345(device, GravityRange.Range08))
{

    var dataCollector = new AsyncSensorDataCollector<Adxl345Reading>(TimeSpan.FromMilliseconds(100), new Adxl345DataSource(sensor));
    dataCollector.Start();
    Console.ReadLine();
}




static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {

        });