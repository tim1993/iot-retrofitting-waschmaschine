using Iot.Device.Adxl345;

namespace WashingIot.Data;

class Adxl345DataSource : ISensorDataSource<Adxl345Reading>
{
    private readonly Adxl345 _sensor;

    public Adxl345DataSource(Adxl345 sensor)
    {
        _sensor = sensor;
    }
    public Task<Adxl345Reading> GetMeasurmentAsync()
    {
        var data = _sensor.Acceleration;
        return Task.FromResult(new Adxl345Reading(data.X, data.Y, data.Z));
    }
}

public record Adxl345Reading(float X, float Y, float Z);