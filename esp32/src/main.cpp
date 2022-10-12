#include <Wifi.h>
#include "AccelerometerManager.h"
#include "OledDisplayManager.h"
#include "InfluxManager.h"
#include "credentials.h"

OledDisplayManager *pOledDisplayManager = new OledDisplayManager();
AccelerometerManager *pAccelerometerManager = new AccelerometerManager();
InfluxManager *pInfluxManager = new InfluxManager();

InfluxManager influxManager = *pInfluxManager;
OledDisplayManager oledDisplayManager = *pOledDisplayManager;
AccelerometerManager accelerometerManager = *pAccelerometerManager;

TaskHandle_t HelperTask;
TaskHandle_t ProcessingTask;

int rssi = 0;
bool isInfluxConnected = false;
const char* ntpServer = "pool.ntp.org";
const long  gmtOffset_sec = 3600;
const int   daylightOffset_sec = 3600;

void initWiFi()
{
  WiFi.mode(WIFI_STA);
  WiFi.begin(WIFI_SSID, WIFI_PW);
  Serial.print("Connecting to WiFi ..");
  while (WiFi.status() != WL_CONNECTED)
  {
    Serial.print('.');
    delay(1000);
  }
  Serial.println(WiFi.localIP());
}

void HelperTaskCode(void *pvParameters)
{
  while (true)
  {
    /* Get a new sensor event */
    sensors_event_t event;
    accelerometerManager.accelerometer.getEvent(&event);

    influxManager.writeAccelerometerData(&event);

    delay(500);
  }
}

void ProcessingTaskCode(void *pvParameters)
{
  while (true)
  {
    /* Get a new sensor event */
    sensors_event_t event;
    accelerometerManager.accelerometer.getEvent(&event);

    accelerometerManager.logAccelerometer(&event);
    oledDisplayManager.showAccelerometerDetails(&event, 2, rssi, isInfluxConnected);

    delay(500);
  }
}

void setup()
{
  Serial.begin(115200);
  accelerometerManager.setupAdxl345();
  oledDisplayManager.setupOledDisplay();
  initWiFi();
  rssi = WiFi.RSSI();
  Serial.println(rssi);
  influxManager.setup();
  isInfluxConnected = influxManager.client.validateConnection();

  //init and get the time
  configTime(gmtOffset_sec, daylightOffset_sec, ntpServer);

  xTaskCreatePinnedToCore(
      HelperTaskCode, /* Task function. */
      "DisplayTask",   /* name of task. */
      10000,           /* Stack size of task */
      NULL,            /* parameter of the task */
      1,               /* priority of the task */
      &HelperTask,    /* Task handle to keep track of created task */
      0);              /* pin task to core 0 */

  xTaskCreatePinnedToCore(
      ProcessingTaskCode, /* Task function. */
      "ProcessingTask",   /* name of task. */
      10000,              /* Stack size of task */
      NULL,               /* parameter of the task */
      1,                  /* priority of the task */
      &ProcessingTask,    /* Task handle to keep track of created task */
      1);                 /* pin task to core 1 */
}

void loop()
{
}