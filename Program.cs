// See https://aka.ms/new-console-template for more information
using System.Device.Spi;
using System.Numerics;
using Iot.Device.Adxl345;

Console.WriteLine("Hello, World!");

SpiConnectionSettings settings = new SpiConnectionSettings(0, 0)
{
    ClockFrequency = Adxl345.SpiClockFrequency,
    Mode = Adxl345.SpiMode
};

var device = SpiDevice.Create(settings);

using (Adxl345 sensor = new Adxl345(device, GravityRange.Range08))
{
    // loop
    while (true)
    {
        // read data
        Vector3 data = sensor.Acceleration;

        Console.WriteLine($"X: {data.X.ToString("0.00")} g");
        Console.WriteLine($"Y: {data.Y.ToString("0.00")} g");
        Console.WriteLine($"Z: {data.Z.ToString("0.00")} g");
        Console.WriteLine();

        // wait for 500ms
        Thread.Sleep(1000);
    }
}