#include <Adafruit_CircuitPlayground.h>
#include <Adafruit_NeoPixel.h>

typedef void (*callback_function)(void);

enum  direction { FORWARD, REVERSE };
enum  pattern { NONE, FULL, INCREMENT, FADE, FLASH };

union stuff
{
  unsigned long value;
  byte bytes[4];
};
struct color
{
  int red;
  int green;
  int blue;
};

struct pixel
{
  color Color1;
  color Color2;
  bool fade=false;
  uint16_t Index;
  uint16_t TotalSteps;
  direction Direction; 
  unsigned long Interval;
  unsigned long lastUpdate;
  bool on=false;
};

pattern Pattern = FADE;
unsigned long Interval;
unsigned long lastUpdate;
pixel Pixel[10];
void (*OnComplete)();

uint32_t Cyan = 0x00FFFF;
uint32_t Black = 0x000000;
uint32_t White = 0xFFFFFF;
uint32_t Red = 0xFF0000;
uint32_t Green = 0x00FF00;
uint32_t Blue = 0x0000FF;
uint32_t Magenta = 0xFF00FF;
uint32_t Yellow = 0xFFFF00;
byte pin = 0;
int wait;
float x;
float y;
float z;
float tempC;
float tempF;
float light;
float sound;
bool left;
bool right;
bool slideSwitch;
void readSensors();
void writeSerial();
void readSerial();
void fade(int i,color color1, color color2, uint16_t steps, uint8_t interval, direction dir = FORWARD);

void setup() {
  // put your setup code here, to run once:
  CircuitPlayground.begin();
  Serial.begin(9600);
  Interval=10;
  lastUpdate=0;
  Pattern=FADE;
  //CircuitPlayground.setPixelColor(0,Black);
      for(int i=0;i<10;i++)
      {
        Pixel[i].Color1.red = Black >> 16 &0xff;
        Pixel[i].Color1.green = Black >> 8 &0xff;
        Pixel[i].Color1.blue = Black & 0xff;
        Pixel[i].Color2.red = Cyan >> 16 &0xff;
        Pixel[i].Color2.green = Cyan >> 8 &0xff;
        Pixel[i].Color2.blue = Cyan &0xff;
        Pixel[i].fade = true;
        Pixel[i].on = true;
        fade(i,Pixel[i].Color1,Pixel[i].Color2,10,1000);
      }
}

void loop() {
  // put your main code here, to run repeatedly:
  Update(doSerial);
  //checkPattern();
  //readSensors();
  //writeSerial();


  
  if(Serial.available()>9)
  {
    readSerial();
  }
  fadeUpdate();
  for(int i=0;i<10;i++)
  {
    if (Pixel[i].on==true&&Pixel[i].fade==false)
      CircuitPlayground.setPixelColor(i,Pixel[i].Color1.red,Pixel[i].Color1.green,Pixel[i].Color1.blue);
  }
}

void turnOnLed(int led,byte r,byte g,byte b)
{
  CircuitPlayground.setPixelColor(led,r,g,b);
}

void readSerial()
{
  byte packet [13];
  for(int i=0;i<13;i++)
    packet[i]=Serial.read();
  byte command = packet[0];

  switch(command)
  {
    case 0xAA:
    {
      byte pin,r,g,b;

      pin = packet[1];
      Pixel[pin].Color1.red = packet[2];
      Pixel[pin].Color1.green = packet[3];
      Pixel[pin].Color1.blue = packet[4];
      Pixel[pin].fade = false;
      Pixel[pin].on = true;
      //for (int i=0;i<9;i++)
      //Serial.read();

      turnOnLed(pin,r,g,b);
      Pattern = NONE;

      break;
    }
    case 0x01:
    {
      pin = packet[1];
      Pixel[pin].Color1.red = packet[2];
      Pixel[pin].Color1.green = packet[3];
      Pixel[pin].Color1.blue = packet[4];
      Pixel[pin].Color2.red = packet[5];
      Pixel[pin].Color2.green = packet[6];
      Pixel[pin].Color2.blue = packet[7];
      stuff waitArray;
      for (int i=0;i<4;i++)
      {
        waitArray.bytes[i]=packet[8+i];
      }
      //Serial.read();
      wait = waitArray.value/255;
      
      fade(pin,Pixel[pin].Color1,Pixel[pin].Color2,255,wait);
      Pixel[pin].fade = true;
      Pixel[pin].on = true;
      Pattern = NONE;
      break;
    }
    case 0x02:
    {
      for(int i=0;i<10;i++)
      {
        ClearPixel(i);
      }
      Pattern = NONE;
      break;
    }
    case 0x03:
    {
      Pattern=FULL;
      for(int i=0;i<10;i++)
      {
        fade(i,Pixel[i].Color1,Pixel[i].Color2,255,4);
        Pixel[i].fade = true;
        Pixel[i].on = true;
      }
      break;
    }
    case 0x04:
    {
      Pattern=INCREMENT;
      break;
    }
    case 0x05:
    {
      Pattern=FADE;
      for(int i=0;i<10;i++)
      {
        Pixel[i].Color1.red = Black >> 24 & 0xFF;
        Pixel[i].Color1.green = Black >> 16 & 0xFF;
        Pixel[i].Color1.blue = Black >> 8 & 0xFF;
        Pixel[i].Color2.red = Cyan >> 24 & 0xFF;
        Pixel[i].Color2.green = Cyan >> 16 & 0xFF;
        Pixel[i].Color2.blue = Cyan >> 8 & 0xFF;
        Pixel[i].fade = true;
        Pixel[i].on = true;
        fade(i,Pixel[i].Color1,Pixel[i].Color2,255,4);
      }
      break;
    }
    case 0x06:
    {
      Pattern=FLASH;
      break;
    }
    default:
      break;
  }
  
}

void writeSerial()
{
  Serial.print(x);
  Serial.print(",");
  Serial.print(y);
  Serial.print(",");
  Serial.print(z);
  Serial.print(",");
  Serial.print(tempC);
  Serial.print(",");
  Serial.print(tempF);
  Serial.print(",");
  Serial.print(light);
  Serial.print(",");
  Serial.print(sound);
  Serial.print(",");
  Serial.print(left);
  Serial.print(",");
  Serial.print(right);
  Serial.print(",");
  Serial.println(slideSwitch);
}

void readSensors()
{
  x = CircuitPlayground.motionX();
  y = CircuitPlayground.motionY();
  z = CircuitPlayground.motionZ();
  tempC = CircuitPlayground.temperature();
  tempF = CircuitPlayground.temperatureF(); 
  light = CircuitPlayground.lightSensor();
  sound = CircuitPlayground.soundSensor();
  left = CircuitPlayground.leftButton();
  right = CircuitPlayground.rightButton();
  slideSwitch = CircuitPlayground.slideSwitch();
}

void ClearPixel(int i)
{
  Pixel[i].Color1.red=0;
  Pixel[i].Color1.green=0;
  Pixel[i].Color1.blue=0;
  Pixel[i].Color2.red=0;
  Pixel[i].Color2.green=0;
  Pixel[i].Color2.blue=0;
  Pixel[i].Index = 0;
  Pixel[i].TotalSteps = 0;
  Pixel[i].Direction = FORWARD; 
  Pixel[i].Interval = 0;
  Pixel[i].lastUpdate = 0;
  Pixel[i].fade=false;
  Pixel[i].on=false;
  CircuitPlayground.setPixelColor(i,Pixel[i].Color1.red,Pixel[i].Color1.green,Pixel[i].Color1.blue);
}

void checkPattern()
{
  switch(Pattern)
  {
    case NONE:
      break;
    case FULL:
    {
      incrementUpdate();
      fadeUpdate();
      Flash();
      break;
    }
    case INCREMENT:
    {
      incrementUpdate();
      break;
    }
    case FADE:
    {
      fadeUpdate();
      break;
    }
    case FLASH:
    {
      Flash();
      break;
    }
    default:
      break;
  }
}

void incrementUpdate()
{
  
}

void Flash()
{
  
}
void Increment(int i)
{
    if (Pixel[i].Direction == FORWARD)
    {
       Pixel[i].Index++;
       if (Pixel[i].Index >= Pixel[i].TotalSteps)
        {
            Pixel[i].Index = Pixel[i].TotalSteps;
            Pixel[i].Direction = REVERSE;
            if (OnComplete != NULL)
            {
                OnComplete(); // call the comlpetion callback
            }
        }
    }
    else // Direction == REVERSE
    {
        --Pixel[i].Index;
        if (Pixel[i].Index <= 0)
        {
            Pixel[i].Index = 0;
            Pixel[i].Direction = FORWARD;
            if (OnComplete != NULL)
            {
                OnComplete(); // call the comlpetion callback
            }
        }
    }
}

void fade(int i, color color1, color color2, uint16_t steps, uint8_t interval, direction dir = FORWARD)
{
    //ActivePattern = FADE;
    Pixel[i].Interval = interval;
    Pixel[i].TotalSteps = steps;
    Pixel[i].Color1 = color1;
    Pixel[i].Color2 = color2;
    Pixel[i].Index = 0;
    Pixel[i].fade=true;
    Pixel[i].lastUpdate=0;
    //Pixel[i].Direction = dir;
}

void fadeUpdate()
{
  uint8_t red;
  uint8_t green;
  uint8_t blue;
  for(int i=0;i<10;i++)
  {
    if (Pixel[i].fade==true)
    {
      if((millis() - Pixel[i].lastUpdate) > Pixel[i].Interval) // time to update
      {
        Pixel[i].lastUpdate=millis();
        int temp = ((Pixel[i].Color1.red * (Pixel[i].TotalSteps - Pixel[i].Index)) + (Pixel[i].Color2.red * Pixel[i].Index)) / Pixel[i].TotalSteps;
        if(temp>255)
          red = 255;
        else
          red = temp;
  
        temp = ((Pixel[i].Color1.green * (Pixel[i].TotalSteps - Pixel[i].Index)) + (Pixel[i].Color2.green * Pixel[i].Index)) / Pixel[i].TotalSteps;
        if(temp>255)
          green = 255;
        else
          green = temp;
  
        temp = ((Pixel[i].Color1.blue * (Pixel[i].TotalSteps - Pixel[i].Index)) + (Pixel[i].Color2.blue * Pixel[i].Index)) / Pixel[i].TotalSteps;
        if(temp>255)
          blue = 255;
        else
          blue = temp;
          
        CircuitPlayground.setPixelColor(i,red, green, blue);
        
        Increment(i);
      }
    }
  }
}

void Update(callback_function pFunc)
{
    if((millis() - lastUpdate) > Interval) // time to update
    {
        lastUpdate = millis();
        pFunc();  
    }
}


void doSerial()
{
  readSensors();
  writeSerial();
}

