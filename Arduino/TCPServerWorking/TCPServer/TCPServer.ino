/*
 Chat  Server
 
 A simple server that distributes any incoming messages to all
 connected clients.  To use telnet to  your device's IP address and type.
 You can see the client's input in the serial monitor as well.
 Using an Arduino Wiznet Ethernet shield. 
 
 Circuit:
 * Ethernet shield attached to pins 10, 11, 12, 13
 * Analog inputs attached to pins A0 through A5 (optional)
 
 created 18 Dec 2009
 by David A. Mellis
 modified 9 Apr 2012
 by Tom Igoe
 
 */

#include <SPI.h>
#include <Ethernet.h>

// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network.
// gateway and subnet are optional:
byte mac[] = {  
  0x90, 0xA2, 0xDA, 0x0F, 0x43, 0xB2 };
//IPAddress ip(169, 254, 60, 110); //169.254.60.110 works on mac
IPAddress ip(192, 168, 137, 2);


//port set to 13000 for tcp comms with c#
EthernetServer server(13000);
boolean alreadyConnected = false; // whether or not the client was connected previously

//control characters
//byte stx[] = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
//byte etx[] = { 0xFE, 0xFE, 0xFE, 0xFE, 0xFE, 0xFE, 0xFE};
char stx[] = { '{', '{', '{', '{', '{', '{', '{' };
char etx[] = { '}', '}', '}', '}', '}', '}', '}' };


//data packet
byte packet[20];
char header[7];
char footer[7];

void setup() {
  // initialize the ethernet device
  Ethernet.begin(mac, ip);
  // start listening for clients
  server.begin();
 // Open serial communications and wait for port to open:
  Serial.begin(9600);
   while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
  }


  Serial.print("Server address:");
  Serial.println(Ethernet.localIP());
}

bool checkHeader(char checkByte[], bool start)
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
  Serial.println(packet[7]);
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
      client.println("Hello, client!"); 
      alreadyConnected = true;
    } 

    while (client.available() > 0) {
      // read the bytes incoming from the client:
      //char thisChar = client.read();
      //if (checkHeader(thisByte, true))
      byte thisByte = 0x00;
      if (client.find(stx))
      {
         Serial.println("Header Found");
         //client.readBytes(header, 7);
         //Serial.println(header);
         /*for (int i = 0; client.available() > 0 && i < 20; i++)
         {
            thisByte = client.read();
            packet[i] = thisByte;
         }*/
         client.read(packet, 20);
         //Serial.print(packet[6]);
         for (int i = 0; i < 20; i++)
         {
            Serial.print(packet[i]);
         }
         client.readBytes(footer, 7);
         if (!checkHeader(footer, false))
         {
           break;
         }
         //client.read(packet, 20);
         
         //processPacket(packet);
      }
      // echo the bytes to the server as well:
      //Serial.write(thisByte);
    }
    // echo the bytes back to the client:
    //Serial.println("\nFinished reading message or client connected!");
    //server.write("Data read!");
  }
}



