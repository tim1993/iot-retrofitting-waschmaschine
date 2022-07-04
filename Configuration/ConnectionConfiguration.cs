namespace WashingIot.Configuration;

public class ConnectionConfiguration
{
    public string IoTHubConnectionString { get; set; } = null!;
    public string InfluxDbHost { get; set; } = null!;

    public string InfluxDbToken { get; set; } = null!;

    public string InfluxDbBucket { get; set; } = null!;

    public string InfluxDbOrg { get; set; } = null!;
}
