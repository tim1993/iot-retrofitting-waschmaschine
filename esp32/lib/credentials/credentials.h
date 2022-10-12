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