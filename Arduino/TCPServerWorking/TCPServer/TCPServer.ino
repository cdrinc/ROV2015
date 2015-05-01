/********************************************************
 * PID Basic Example
 * Reading analog input 0 to control analog PWM output 3
 *******************************************************

#include <PID_v1.h>

//Define Variables we'll be connecting to
double Setpoint, Input, Output;

//Specify the links and initial tuning parameters
PID myPID(&Input, &Output, &Setpoint,2,5,1, DIRECT);

void setup()
{
  //initialize the variables we're linked to
  Input = analogRead(0);
  Setpoint = 100;

  //turn the PID on
  myPID.SetMode(AUTOMATIC);
}

void loop()
{
  Input = analogRead(0);
  myPID.Compute();
  analogWrite(3,Output);
}
*/
#include <SPI.h>
#include <Ethernet.h>
#include <Wire.h>
#include <MS5803_I2C.h>
#include <stdio.h>

// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network.
// gateway and subnet are optional:
byte mac[] = {  
  0x90, 0xA2, 0xDA, 0x0F, 0x43, 0xB2 };
IPAddress ip(169, 254, 60, 110); //169.254.60.110 works on mac
//IPAddress ip(169, 254, 180, 60); //169.254.180.60 works on mac port 2
//IPAddress ip(192, 168, 137, 2); //works on windows


//port set to 13000 for tcp comms with c#
EthernetServer server(13000);
boolean alreadyConnected = false; // whether or not the client was connected previously
EthernetClient client;
EthernetClient altClient;

//control characters
byte stx[] = { 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B};
byte etx[] = { 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D};
byte sensorByte =  0x00;
byte motorByte = 0x01;
byte stringByte = 0x02;

//byte exitSafe[] = { 0xAA, 0x0D, 0x03 };
//char stx[] = { '{', '{', '{', '{', '{', '{', '{' };
//char etx[] = { '}', '}', '}', '}', '}', '}', '}' };

//byte values of connected controllers
byte controllers[] = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };

//data packet
byte motorPacket[10];
byte sensorPacket[10];
byte header[7];
byte footer[7];

// pressure sensor address
// ADDRESS_HIGH = 0x76
// ADDRESS_LOW = 0x77;

MS5803 pSensor(ADDRESS_HIGH);

void setup() {
  pinMode(8, INPUT);
  pinMode(9, OUTPUT);
  digitalWrite(9, HIGH);
  // initialize the ethernet device
  Ethernet.begin(mac, ip);
  // start listening for clientss
  server.begin();
 // Open serial communications and wait for port to open:
  Serial.begin(115200);
   while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
  }
  
  Serial.print("Server address:");
  Serial.println(Ethernet.localIP());
  
  pSensor.reset();
  pSensor.begin();
}

bool checkHeader(byte checkByte[], bool start)
{
  if (start)
  {
    if (checkByte == etx)
      return true; 
  }
  else if(!start)
  {
    if (checkByte == stx)
      return true;
  }
  return false;
}

void exitSafeStart()
{
  byte exitSafe[] = { 0xAA, 0x00, 0x03 };
  for (int i = 0; i < sizeof(controllers); i++)
  {
    exitSafe[1] = controllers[i];
    Serial.write(exitSafe, 3);
  }
  //delay(25);
}

void processPacket(byte packet[])
{
  exitSafeStart();
  byte deviceNumber; //lets order the device numbers based on the order of bytes in the packet to reduce variable usage and make logic easier
  byte val;
  byte controllerPacket[5];
  //handles the four translate motors, which are the first four in the controllers array
  for (int i = 0; i < 5 && i < sizeof(controllers); i++)
  {
    deviceNumber = controllers[i];
    val = packet[i];
    
    controllerPacket[0] = 0xAA; //auto-detect baud rate
    controllerPacket[1] = deviceNumber; //device number
    //insert logic to handle direction based on device number
    //this is just an implementation of the forward/back ls stick so we know 1-127 is neg and 128-255 is pos
    if (val == 0x00)
    {
      controllerPacket[2] = 0x05;
      controllerPacket[3] = 0x00;
      controllerPacket[4] = 0x00;
    }
    else
    {
      //handles setting the speed if val is a byte (0-255) e.g. bound to packet[0] (LSY)
      byte dir = val <= 126 ? 0x06 : 0x05;
      byte minInitialRange = dir == 0x06 ? 1 : 128; //gets the initial min for mapping based on direction
      byte maxInitialRange = dir == 0x06 ? 127 : 255; //gets the initial max for mapping based on direction
      int motorSpeed = map(val,minInitialRange,maxInitialRange,0,3200);
      //this handles setting the motor speed if val is binary (one or zero) e.g. bound to packet[6] (A)
      //byte dir = 0x05;
      //int motorSpeed = val == 0 ? 0 : 1000;
      controllerPacket[2] = dir;
      controllerPacket[3] = motorSpeed % 32; //how to get the first 5 bits
      controllerPacket[4] = motorSpeed / 32; //how to get the last 7 bits
    }
    Serial.write(controllerPacket, 5);
  }
  for (int i = 5; i < 8 && i < sizeof(controllers); i++)
  {
    deviceNumber = controllers[i];
    val = packet[i];
    
    controllerPacket[0] = 0xAA;
    controllerPacket[1] = deviceNumber;
    
    if (val == 0)
    {
      controllerPacket[2] = 0x05;
      controllerPacket[3] = 0x00;
      controllerPacket[4] = 0x00;
    }
    else
    {
      byte dir = val == 1 ? 0x06 : 0x05;
      int motorSpeed = 3200;
      controllerPacket[2] = dir;
      controllerPacket[3] = motorSpeed % 32; //how to get the first 5 bits
      controllerPacket[4] = motorSpeed / 32; //how to get the last 7 bits
    }
    Serial.write(controllerPacket, 5);
  }
  for (int i = 9; i < 10; i++)
  {
    deviceNumber = controllers[i];
    val = packet[i];
    
    controllerPacket[0] = 0xAA; //auto-detect baud rate
    controllerPacket[1] = deviceNumber; //device number
    //insert logic to handle direction based on device number
    //this is just an implementation of the forward/back ls stick so we know 1-127 is neg and 128-255 is pos
    if (val == 0x00)
    {
      controllerPacket[2] = 0x05;
      controllerPacket[3] = 0x00;
      controllerPacket[4] = 0x00;
    }
    else
    {
      //handles setting the speed if val is a byte (0-255) e.g. bound to packet[0] (LSY)
      byte dir = val <= 126 ? 0x06 : 0x05;
      byte minInitialRange = dir == 0x06 ? 1 : 128; //gets the initial min for mapping based on direction
      byte maxInitialRange = dir == 0x06 ? 127 : 255; //gets the initial max for mapping based on direction
      int motorSpeed = map(val,minInitialRange,maxInitialRange,0,3200);
      //this handles setting the motor speed if val is binary (one or zero) e.g. bound to packet[6] (A)
      //byte dir = 0x05;
      //int motorSpeed = val == 0 ? 0 : 1000;
      controllerPacket[2] = dir;
      controllerPacket[3] = motorSpeed % 32; //how to get the first 5 bits
      controllerPacket[4] = motorSpeed / 32; //how to get the last 7 bits
    }
    Serial.write(controllerPacket, 5);
  }
  
  /*for (int i = 0; i < sizeof(controllers); i++)
  {
    deviceNumber = controllers[i]; //lets order the device numbers based on the order of bytes in the packet to reduce variable usage and make logic easier
    //just for now
    val = packet[i];
        
    controllerPacket[0] = 0xAA; //auto-detect baud rate
    controllerPacket[1] = deviceNumber; //device number
    controllerPacket[2] = command; //command byte
    //insert logic to handle direction based on device number
    //this is just an implementation of the forward/back ls stick so we know 1-127 is neg and 128-255 is pos
    if (val == 0x00)
    {
      controllerPacket[2] = 0x05;
      controllerPacket[3] = 0x00;
      controllerPacket[4] = 0x00;
    }
    else
    {
      //handles setting the speed if val is a byte (0-255) e.g. bound to packet[0] (LSY)
      byte dir = val <= 126 ? 0x06 : 0x05;
      byte minInitialRange = dir == 0x06 ? 1 : 128; //gets the initial min for mapping based on direction
      byte maxInitialRange = dir == 0x06 ? 127 : 255; //gets the initial max for mapping based on direction
      int motorSpeed = map(val,minInitialRange,maxInitialRange,0,3200);
      //this handles setting the motor speed if val is binary (one or zero) e.g. bound to packet[6] (A)
      //byte dir = 0x05;
      //int motorSpeed = val == 0 ? 0 : 1000;
      controllerPacket[2] = dir;
      controllerPacket[3] = motorSpeed % 32; //how to get the first 5 bits
      controllerPacket[4] = motorSpeed / 32; //how to get the last 7 bits
    }
    // Serial.print(dir);
    Serial.write(controllerPacket, 5);
  }*/
}

void serializeFloat(float val, byte* array)
{
  union
  {
    float f;
    byte buff[4];
  } u;
  
  u.f = val;
  memcpy(array, u.buff, 4);
}

void sendSensorPacket(EthernetClient& client)
{
  byte sendPacket[27];
  byte pData[4];
  for (int i = 0; i < 7; i++)
  {
     sendPacket[i] = '{'; 
  }
  sendPacket[7] = sensorByte;
  
  serializeFloat(pSensor.getPressure(ADC_4096), pData);
  for (int i = 0; i < 4; i++)
  {
    sendPacket[i + 8] = pData[i];
  }
  
  // voltage code goes here
  for (int i = 0; i < 4; i++)
  {
    sendPacket[i + 12] = 0;
  }
  
  // length code goes here
  for (int i = 0; i < 4; i++)
  {
    sendPacket[i + 16] = 0;
  }
  
  for (int i = 0; i < 7; i++)
  {
     sendPacket[i + 20] = '}'; 
  }
  
  client.write(sendPacket, 27);
}

void sendMotorPacket(byte motorData[], EthernetClient& client)
{
  byte sendPacket[25];
  for (int i = 0; i < 7; i++)
  {
     sendPacket[i] = '{'; 
  }
  sendPacket[7] = motorByte;
  for (int i = 0; i < 10; i++)
  {
     sendPacket[i + 8] = motorData[i];
  }
  for (int i = 0; i < 7; i++)
  {
     sendPacket[i + 18] = '}'; 
  }
  
  client.write(sendPacket, 25);
}
  

void loop() {
  // wait for a new client:
  if (!client || server.available())
  {
    client = server.available();
  }

  // when the client sends the first byte, say hello:
  if (client.connected()) {
    if (!alreadyConnected) {
      Serial.println("We have a new client"); 
      alreadyConnected = true;
      //client.print("{{{{{{{");
      //client.write(stringByte);
      //client.print("Client connected}}}}}}}");
      exitSafeStart();
    }

    while (client.available() > 0) {
      // read the bytes incoming from the client:
      byte thisByte = 0x00;
      if (client.find("{{{{{{{"))
      {
         thisByte = client.read();
         if (thisByte == motorByte)
         {
           for (int i = 0; client.available() > 0 && i < 10; i++)
           {
              thisByte = client.read();
              motorPacket[i] = thisByte;
           }
           for (int i = 0; client.available() > 0 && i < 7; i++)
           {
              thisByte = client.read();
              footer[i] = thisByte;
           }
           sendMotorPacket(motorPacket, client);
           processPacket(motorPacket);
         }
         /*else if (thisByte == 0x02)
         {
           client.print("{{{{{{{");
           client.write(stringByte);
           client.print("Throwing Error...}}}}}}}");
           pinMode(8, OUTPUT);
           digitalWrite(8, HIGH);
           delay(30);
           pinMode(8, INPUT);
         }*/
      }
      sendSensorPacket(client);
    }
  }
}
