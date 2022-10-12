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

## ESP32

If you have an ESP32 Device you can take a look at the [ESP32 Folder](/esp32).   
The sources are ready to use with [PlattformIO](https://platformio.org/).   
To use it with VSCode please take a look at [PlatformIO IDE for VSCode](https://docs.platformio.org/en/latest/integration/ide/vscode.html).

### Build & run
Provide missing libraries in lib folder. I used for that Arduino IDE to install the missing dependencies.   
You need to provide the following settings in the credentials.h file:

```c
#ifndef CREDENTIALS_H
#define CREDENTIALS_H

#define WIFI_SSID "<YOUR_WIFI_SSID>"
#define WIFI_PW "<YOUR_WIFI_PASSWORD>"

// InfluxDB v2 server url, e.g. https://eu-central-1-1.aws.cloud2.influxdata.com (Use: InfluxDB UI -> Load Data -> Client Libraries)
#define INFLUXDB_URL "http://<ip>:<port>"
// InfluxDB v2 server or cloud API token (Use: InfluxDB UI -> Data -> API Tokens -> <select token>)
#define INFLUXDB_TOKEN "<INFLUXDB_TOKEN>"
// InfluxDB v2 organization id (Use: InfluxDB UI -> User -> About -> Common Ids )
#define INFLUXDB_ORG "Digitalisierte Waschmaschine"
// InfluxDB v2 bucket name (Use: InfluxDB UI ->  Data -> Buckets)
#define INFLUXDB_BUCKET "IoT-Waschmaschine"

#endif
```

Run F1 -> PlattformIO Build   
Run F1 -> PlattformIO Upload