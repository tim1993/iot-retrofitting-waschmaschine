#include <Adafruit_ADXL345_U.h>
#include "AccelerometerManager.h"

/* Assign a unique ID to this sensor at the same time */
Adafruit_ADXL345_Unified AccelerometerManager::accelerometer = Adafruit_ADXL345_Unified(1337);

void AccelerometerManager::setupAdxl345()
{
  /* Initialise the sensor */
  if (!accelerometer.begin())
  {
    /* There was a problem detecting the ADXL345 ... check your connections */
    Serial.println("Ooops, no ADXL345 detected ... Check your wiring!");
    while (1)
      ;
  }
  else
  {
    Serial.println("ADXL345 detection successful!");
  }

  accelerometer.setRange(ADXL345_RANGE_16_G);
  // displaySetRange(ADXL345_RANGE_8_G);
  // displaySetRange(ADXL345_RANGE_4_G);
  // displaySetRange(ADXL345_RANGE_2_G);
  displaySensorDetails();
}

void AccelerometerManager::displaySensorDetails()
{
  sensor_t sensor;
  accelerometer.getSensor(&sensor);
  Serial.println("------------------------------------");
  Serial.print("Sensor: ");
  Serial.println(sensor.name);
  Serial.print("Driver Ver: ");
  Serial.println(sensor.version);
  Serial.print("Unique ID: ");
  Serial.println(sensor.sensor_id);
  Serial.print("Max Value: ");
  Serial.print(sensor.max_value);
  Serial.println(" m/s^2");
  Serial.print("Min Value: ");
  Serial.print(sensor.min_value);
  Serial.println(" m/s^2");
  Serial.print("Resolution: ");
  Serial.print(sensor.resolution);
  Serial.println(" m/s^2");
  Serial.println("------------------------------------");
  Serial.println("");
  delay(500);
}

void AccelerometerManager::logAccelerometer(sensors_event_t *pEvent)
{
  sensors_event_t event = *pEvent;
  /* Display the results (acceleration is measured in m/s^2) */
  Serial.print("X: ");
  Serial.print(event.acceleration.x);
  Serial.print(" ");
  Serial.print("Y: ");
  Serial.print(event.acceleration.y);
  Serial.print(" ");
  Serial.print("Z: ");
  Serial.print(event.acceleration.z);
  Serial.print(" ");
  Serial.println("m/s^2 ");
}