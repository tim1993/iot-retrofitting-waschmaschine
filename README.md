# iot-retrofitting-waschmaschine

Welcome to our sample code for a vibration-based activity detection of a washing machine. We used a Raspberry Pi 3B and an ADXL345 accelerometer for vibration sensing. The here provided software sends the current detected state to the iothub after state transition.

## Getting started

### Setup influx db

For local analysis we transmit relevant data like ADXL345 raw values + calculated velocity variance to an influxdb. To get started you can use the docker-compose.yaml provided in scripts/influxdb. Its' configuration matches with the here exposed settings regarding influxdb in appsettings.json

### Configure the solution

You need to provide the following settings in the appsettings.json and it must be present on the raspberry pi

```json
{
  "ConnectionConfiguration": {
    "IoTHubConnectionString": "<connectionString>",
    "InfluxDbHost": "http://localhost:8086",
    "InfluxDbToken": "demo",
    "InfluxDbBucket": "washingmachine",
    "InfluxDbOrg": "medialesson"
  }
}
```

### Build & run

For deployment you can use scripts/publish-ssh.ps1 in order to build and transfer the solution to your raspberry pi via SSH/SCP or take it as an example for your manual deployment. You need to modify at least the variable with your corresponding hostname `$serverAddress`.

After that you can navigate to the deployment folder on the raspberry pi and run `dotnet run washing-iot.dll` and it will start the vibration monitoring.
