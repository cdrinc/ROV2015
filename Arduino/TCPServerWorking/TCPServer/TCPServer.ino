#include <SPI.h>
#include <Ethernet.h>

// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network.
// gateway and subnet are optional:
byte mac[] = {  
  0x90, 0xA2, 0xDA, 0x0F, 0x43, 0xB2 };
//IPAddress ip(169, 254, 60, 110); //169.254.60.110 works on mac
//IPAddress ip(169, 254, 180, 60); //169.254.180.60 works on mac port 2
IPAddress ip(192, 168, 137, 2); //works on windows


//port set to 13000 for tcp comms with c#
EthernetServer server(13000);
boolean alreadyConnected = false; // whether or not the client was connected previously
EthernetClient client;
EthernetClient altClient;

//control characters
byte stx[] = { 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B};
byte etx[] = { 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D};
byte testByte =  0x00;
byte packetByte = 0x01;
byte stringByte = 0x02;

//byte exitSafe[] = { 0xAA, 0x0D, 0x03 };
//char stx[] = { '{', '{', '{', '{', '{', '{', '{' };
//char etx[] = { '}', '}', '}', '}', '}', '}', '}' };

//byte values of connected controllers
byte controllers[] = { 0x0D, 0x0E };

//data packet
byte packet[20];
byte header[7];
byte footer[7];

void setup() {
  pinMode(8, INPUT);
  pinMode(9, OUTPUT);
  pinMode(7, OUTPUT);
  digitalWrite(9, HIGH);
  digitalWrite(7, HIGH);
  // initialize the ethernet device
  Ethernet.begin(mac, ip);
  // start listening for clientss
  server.begin();
 // Open serial communications and wait for port to open:
  Serial.begin(9600);
   while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
  }


  Serial.print("Server address:");
  Serial.println(Ethernet.localIP());
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
  delay(25);
}

void processPacket(byte packet[])
{
  byte deviceNumber; //lets order the device numbers based on the order of bytes in the packet to reduce variable usage and make logic easier
  byte command; //motor forward
  byte val = packet[0];
  byte controllerPacket[5];
  for (int i = 0; i < sizeof(controllers); i++)
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
      /*byte dir = 0x05;
      int motorSpeed = val == 0 ? 0 : 1000;*/
      controllerPacket[2] = dir;
      controllerPacket[3] = motorSpeed % 32; //how to get the first 5 bits
      controllerPacket[4] = motorSpeed / 32; //how to get the last 7 bits
    }
   // Serial.print(dir);
    Serial.write(controllerPacket, 5);
  }
}

void sendTestPacket(byte data[], EthernetClient& client)
{
  byte sendPacket[35];
  for (int i = 0; i < 7; i++)
  {
     sendPacket[i] = '{'; 
  }
  sendPacket[7] = testByte;
  for (int i = 0; i < 20; i++)
  {
     sendPacket[i + 8] = data[i];
  }
  for (int i = 0; i < 7; i++)
  {
     sendPacket[i + 28] = '}'; 
  }
  
  client.write(sendPacket, 35);
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
      client.print("{{{{{{{");
      client.write(stringByte);
      client.print("Client connected}}}}}}}");
      exitSafeStart();
    }
    
    if (digitalRead(8) == HIGH)
    {
      client.print("{{{{{{{");
      client.write(stringByte);
      client.print("Error Detected}}}}}}}");
      exitSafeStart();
    }

    while (client.available() > 0) {
      // read the bytes incoming from the client:
      byte thisByte = 0x00;
      if (client.find("{{{{{{{"))
      {
         thisByte = client.read();
         if (thisByte == 0x00)
         {
           for (int i = 0; client.available() > 0 && i < 11; i++)
           {
              thisByte = client.read();
              packet[i] = thisByte;
           }
           for (int i = 0; client.available() > 0 && i < 7; i++)
           {
              thisByte = client.read();
              footer[i] = thisByte;
           }
           
           sendTestPacket(packet, client);
           processPacket(packet);
         }
         else if (thisByte == 0x01)
         {
           for (int i = 0; client.available() > 0 && i < 20; i++)
           {
              thisByte = client.read();
              packet[i] = thisByte;
           }
           for (int i = 0; client.available() > 0 && i < 7; i++)
           {
              thisByte = client.read();
              footer[i] = thisByte;
           }
           sendTestPacket(packet, client);
           processPacket(packet);
         }
         else if (thisByte == 0x02)
         {
           client.print("{{{{{{{");
           client.write(stringByte);
           client.print("Throwing Error...}}}}}}}");
           pinMode(8, OUTPUT);
           digitalWrite(8, HIGH);
           delay(30);
           pinMode(8, INPUT);
         }
      }
    }
  }
}



