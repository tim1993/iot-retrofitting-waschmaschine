#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <Adafruit_ADXL345_U.h>

class OledDisplayManager
{
    static Adafruit_SSD1306 display;

public:
    void setupOledDisplay();
    void showAccelerometerDetails(sensors_event_t *pEvent,  int textSize, int rssi, bool isInfluxConnected);
    void showSignalStrength(int rssi);
    void printLocalTime();
};