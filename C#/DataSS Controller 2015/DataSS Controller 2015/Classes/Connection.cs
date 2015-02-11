using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DataSS_Controller_2015.Classes
{
    public class TcpConnection
    {
        public TcpClient Client;
        public StreamWriter Writer;
        public NetworkStream Stream;

        //"Standard Header" byte array that marks the beginning of each message
        //is 7 bytes because the packet structure includes all values that can be above 1 (RS, RT, etc.), of which there are 6, consecutively
        //that way it shouldn't be be able to be replicated in the packet itself
        private byte[] stx = { 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B };
        public byte[] Header
        {
            get { return stx; }
            set { }
        }
        //"Standard Footer" end of each message
        //same as above
        public byte[] etx = { 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D };
        public byte[] Footer
        {
            get { return etx; }
            set { }
        }

        //blank byte to send as handshake
        private byte[] blankData = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //this holds the data each time ReadPacket() is called
        private byte[] packet = new byte[34];

        public TcpConnection(string ipAddress, int port)
        {
            try
            {
                // Create a TcpClient. 
                // Note, for this client to work you need to have a TcpServer  
                // connected to the same address as specified by the server, port 
                // combination.
                Client = new TcpClient();
                Client.Connect(ipAddress, port);

                // Get a client stream for reading and writing. 
                // Stream stream = client.GetStream();
                Stream = Client.GetStream();

                //create a writer for the network stream
                Writer = new StreamWriter(Stream);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("It's fucked.\n\n"+ex.Message, "Fucked it is.", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public void Close()
        {
            Writer.Close();
            Client.Close();
        }

        /// <summary>
        /// Searches the stream for a byte array (buffer), reads the buffer, and advances the stream to whatever follows.
        /// </summary>
        /// <param name="buffer">Byte array to search for.</param>
        /// <returns>Returns a boolean value that indicates whether or not buffer was found in the stream.</returns>
        private bool Find(byte[] buffer)
        {
            int i = 0;
            List<byte> data = new List<byte>();
            byte[] dataArray;
            while (Stream.DataAvailable && i < 100)
            {
                i++;
                data.Add((byte)Stream.ReadByte());
                if (data.Count > buffer.Length)
                    data.RemoveAt(0);
                dataArray = data.ToArray();
                if (dataArray.SequenceEqual(buffer))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Exchanges a blank packet with the connected device.
        /// </summary>
        public void Handshake()
        {
            SendPacket(blankData);
        }

        /// <summary>
        /// Determins whether data is available to be read.
        /// </summary>
        /// <returns>Returns a boolean value that indicates whether or not data is available.</returns>
        public bool DataAvailable()
        {
            return Stream.DataAvailable;
        }

        /// <summary>
        /// Sends a string to the connected device.
        /// </summary>
        /// <param name="message">String to send.</param>
        public void Send(string message)
        {
            Writer.Write(message);
            Writer.Flush();
        }

        /// <summary>
        /// Sends a single byte to the connected device.
        /// </summary>
        /// <param name="message">Single byte to send.</param>
        private void Send(byte message)
        {
            byte[] toSend = { message };
            Stream.Write(toSend, 0, 1);
        }

        /// <summary>
        /// Sends a byte array to the connected device.
        /// </summary>
        /// <param name="message">Byte array to send.</param>
        private void Send(byte[] message)
        {
            Stream.Write(message, 0, message.Length);
        }

        /// <summary>
        /// Sends a packet, consisting of a  header, data, and footer, to the connected device.
        /// </summary>
        /// <param name="data">Byte array of data to send. NB: at this time, packet MUST be 20 bytes.</param>
        public void SendPacket(byte[] data)
        {
            Header.CopyTo(packet, 0);
            data.CopyTo(packet, Header.Length);
            Footer.CopyTo(packet, Header.Length + data.Length);
            Send(packet);
        }

        /// <summary>
        /// Reads all data available and returns a string representation. NB: Deprecated in favor of ReadPacket()
        /// </summary>
        /// <returns>Returns a string representation of the bytes read from the stream.</returns>
        public string ReadAllAvailable()
        {
            List<byte> data = new List<byte>();
            while (Stream.DataAvailable)
                data.Add((byte)Stream.ReadByte());
            return System.Text.Encoding.ASCII.GetString(data.ToArray());
        }

        /// <summary>
        /// Reads a full packet, consisting of a header, data, and a footer, from the stream.
        /// </summary>
        /// <returns>Returns a string representation of the data suitable for display in the UI.</returns>
        public string ReadPacket()
        {
            int i = 0;
            List<byte> data = new List<byte>();
            List<byte> footerList = Footer.ToList<byte>();

            if (Find(Header))
            {
                System.Threading.Thread.Sleep(2);
                while (Stream.DataAvailable)
                {
                    data.Add((byte)Stream.ReadByte());
                    if (data.ContainsSequence(footerList))
                    {
                        foreach (byte bracket in footerList)
                        {
                            data.Remove(bracket);
                        }
                        break;
                    }
                }
            }

            string parsedData = "";
            if (data.Count == 20 && data[7] < 2)
            {
                foreach (byte b in data)
                {
                    parsedData += b.ToString();
                    parsedData += "|";
                }
                parsedData = parsedData.Remove(parsedData.Length - 1);
            }
            else
            {
                parsedData = System.Text.Encoding.ASCII.GetString(data.ToArray());
            }

            return parsedData;
        }
    }
}
