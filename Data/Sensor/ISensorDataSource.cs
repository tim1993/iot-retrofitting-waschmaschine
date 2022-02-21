namespace WashingIot.Data;

public interface ISensorDataSource<T> 
{
    Task<T> GetMeasurmentAsync();
}