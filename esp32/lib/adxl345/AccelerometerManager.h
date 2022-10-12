#include <Adafruit_ADXL345_U.h>

class AccelerometerManager
{
public:
    static Adafruit_ADXL345_Unified accelerometer;

    void setupAdxl345();
    void displaySensorDetails();
    void logAccelerometer(sensors_event_t *pEvent);
};