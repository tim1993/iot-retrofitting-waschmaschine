#include <InfluxDbClient.h>
#include <Adafruit_ADXL345_U.h>
#include "credentials.h"

class InfluxManager
{
public:
    static InfluxDBClient client;
    
    void setup();
    void writeAccelerometerData(sensors_event_t *pEvent);
};