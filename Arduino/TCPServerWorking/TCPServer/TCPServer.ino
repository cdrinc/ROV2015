#include <SPI.h>
#include <Ethernet.h>

// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network.
// gateway and subnet are optional:
byte mac[] = {  
  0x90, 0xA2, 0xDA, 0x0F, 0x43, 0xB2 };
//IPAddress ip(169, 254, 60, 110); //169.254.60.110 works on mac
IPAddress ip(169, 254, 180, 60); //169.254.180.60 works on mac port 2
//IPAddress ip(192, 168, 137, 2);


//port set to 13000 for tcp comms with c#
EthernetServer server(13000);
boolean alreadyConnected = false; // whether or not the client was connected previously

//control characters
byte stx[] = { 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B};
byte etx[] = { 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D};
//char stx[] = { '{', '{', '{', '{', '{', '{', '{' };
//char etx[] = { '}', '}', '}', '}', '}', '}', '}' };


//data packet
byte packet[20];
byte header[7];
byte footer[7];

void setup() {
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

void processPacket(byte packet[])
{
  int i = 0;
  byte deviceNumber = 0x0D; //lets order the device numbers based on the order of bytes in the packet to reduce variable usage and make logic easier
  byte val = packet[i];
  Serial.print(0x85); //command byte
  Serial.print(deviceNumber); //device number
  //insert logic to handle direction based on device number
  //this is just an implementation of the forward/back ls stick so we know 0-126 is neg and 127-255 is pos
  byte dir = val <= 126 ? 0x06 : 0x05;
  byte minInitialRange = dir == 0x06 ? 0 : 128; //gets the initial min for mapping based on direction
  byte maxInitialRange = dir == 0x06 ? 126 : 255; //gets the initial max for mapping based on direction
  int motorSpeed = map(val,minInitialRange,maxInitialRange,0,3200);
  motorSpeed = val == 127 ? 0 : val;
  byte speed1 = motorSpeed % 32; //how to get the first 5 bits
  byte speed2 = motorSpeed / 32; //how to get the last 7 bits
  Serial.print(dir);
  Serial.print(speed1);
  Serial.print(speed2);
  //Serial.println(packet[7]);
}

void sendData(byte data[], EthernetClient& client)
{
  byte sendPacket[14 + sizeof(data)];
  for (int i = 0; i < 7; i++)
  {
     sendPacket[i] = stx[i]; 
  }
  for (int i = 0; i < sizeof(data); i++)
  {
     sendPacket[i + 7] = data[i];
  }
  for (int i = 0; i < 7; i++)
  {
     sendPacket[i + 7 + sizeof(data)] = etx[i]; 
  }
  
  client.print((char*)sendPacket);
}
  

void loop() {
  // wait for a new client:
  EthernetClient client = server.available();

  // when the client sends the first byte, say hello:
  if (client) {
    if (!alreadyConnected) {
      // clead out the input buffer:
      // client.flush();    
      Serial.println("We have a new client");
      //byte test[] = { 0x32, 0x33 };
      //sendData(test, client);
      //client.print((char*)stx);
      //client.print((char*)test);
      //client.print((char*)etx); 
      alreadyConnected = true;
      client.print("{{{{{{{Client connected}}}}}}}");
    } 

    while (client.available() > 0) {
      // read the bytes incoming from the client:
      //char thisChar = client.read();
      byte thisByte = 0x00;
      if (client.find((char*)stx))
      {
         //Serial.println("Header Found");
         //client.println("Header Found");
         //sendData((byte*)"Header Found", client);
         //client.readBytes(header, 7);
         //Serial.println(header);
         /*for (int i = 0; client.available() > 0 && i < 7; i++)
         {
            thisByte = client.read();
            header[i] = thisByte;
         }*/
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
         //Serial.println((char*)header);
         /*for (int i = 0; i < 20; i++)
         {
            //Serial.print(packet[i]);
            //Serial.print('|');
            client.print( packet[i]);
            client.print('|');
         }*/
         //client.write('{{{{{{{' + packet + '}}}}}}}', 34);
         client.write("{{{{{{{");
         client.write(packet, 20);
         client.write("}}}}}}}");
         //Serial.println("");
         //client.println("{{{{{{{}}}}}}}");
         /*for (int i = 0; i < 7; i++)
         {
            //Serial.print(footer[i]);
            //Serial.print('|');
            client.print(footer[i]);
            client.print('|');
         }*/
         //Serial.println("");
         //client.println("");
         //Serial.println((char*)footer);
         //client.readBytes(footer, 7);
         /*if ((char*)footer == (char*)etx)
         {
           //Serial.println("Footer Found");
           client.println("{{{{{{{Footer Found}}}}}}}");
         }/*
         /*if (checkHeader(footer, false))
         {
           Serial.println("Footer Found");
         }*/
         //client.read(packet, 20);
         
         processPacket(packet);
      }
      // echo the bytes to the server as well:
      //Serial.write(thisByte);
    }
    // echo the bytes back to the client:
    //Serial.println("\nFinished reading message or client connected!");
    //server.write("Data read!");
  }
}



