namespace WashingIot.Data;

interface ISensorDataSource<T> 
{
    Task<T> GetMeasurmentAsync();
}