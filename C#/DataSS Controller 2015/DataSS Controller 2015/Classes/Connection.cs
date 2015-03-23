using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    /// <summary>
    /// Encapsulates a TcpClient and provides various methods to read, write, and interpret data.
    /// </summary>
    public class TcpConnection
    {
        private TcpClient client;
        private NetworkStream stream;

        private string ipAddress;
        private int port;

        // production, test, and general data bytes, respectively
        private byte prodByte = 0x00;
        private byte testByte = 0x01;
        private byte stringByte = 0x02;

        // blank byte to send as handshake
        private byte[] blankData = { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        // this holds the data each time ReadPacket() is called
        //private byte[] packet = new byte[35];
        //private List<byte> packet;

        private byte[] stx = { 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B };

        private byte[] etx = { 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D };

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnection"/> class and sets the IP Address and Port of the connection.
        /// </summary>
        /// <param name="ipAddress">IP Address to connect to.</param>
        /// <param name="port">Port to connect to.</param>
        public TcpConnection(string ipAddress, int port)
        {
            // Create a TcpClient
            this.client = new TcpClient();
            this.ipAddress = ipAddress;
            this.port = port;

            this.client.SendTimeout = 1000;

            //packet = new List<byte>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the underlying TcpClient has an active connection.
        /// </summary>
        public bool Connected
        {
            get { return client.Connected; }
            set { }
        }

        // "Standard Header" byte array that marks the beginning of each message
        // is 7 bytes because the packet structure includes all values that can be above 1 (RS, RT, etc.), of which there are 6, consecutively
        // that way it shouldn't be be able to be replicated in the packet itself

        /// <summary>
        /// Gets or sets the standard header to send at the beginning of each message
        /// </summary>
        public byte[] Header
        {
            get { return this.stx; }
            set { }
        }

        /// <summary>
        /// Gets or sets the standard footer to send at the end of each message
        /// </summary>
        public byte[] Footer
        {
            get { return this.etx; }
            set { }
        }

        /// <summary>
        /// Closes the active connection.
        /// </summary>
        public void Close()
        {
            client.Close();
        }

        /// <summary>
        /// Attempts to connect to the indicated IP address and port.
        /// </summary>
        /// <param name="success">A boolean value, passed by reference, indicating whether or not the operation was successful.</param>
        /// <param name="errorMessage">A string, passed by reference, containing any error message.</param>
        public void Connect(out bool success, out string errorMessage)
        {
            try
            {
                // Note, for this client to work you need to have a TcpServer  
                // connected to the same address as specified by the server, port 
                // combination.
                client.Connect(ipAddress, port);

                // Get a client stream for reading and writing. 
                stream = client.GetStream();

                success = true;
                errorMessage = null;
                return;
            }
            catch (Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
                return;
            }
        }

        /// <summary>
        /// Exchanges a blank packet with the connected device.
        /// </summary>
        /// <param name="success">A boolean value, passed by reference, indicating whether or not the operation was successful.</param>
        /// <param name="errorMessage">A string, passed by reference, containing any error message.</param>
        public void Handshake(out bool success, out string errorMessage)
        {
            SendPacket(blankData, out success, out errorMessage);
            return;
        }

        /// <summary>
        /// Determines whether data is available to be read.
        /// </summary>
        /// <returns>Returns a boolean value that indicates whether or not data is available.</returns>
        public bool DataAvailable()
        {
            try
            {
                return stream.DataAvailable;
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException) { }
                else
                {
                    System.Windows.Forms.MessageBox.Show("An exception has occured while attempting to read data from the connection:" + ex.Message);
                }

                return false;
            }
        }

        /// <summary>
        /// Reads a packet from the microcontroller and return it encased in the appropriate type.
        /// </summary>
        /// <returns>Returns a ReceivedData object that can be converted into a string for display.</returns>
        public ReceivedData GetResponse()
        {
            List<byte> data = new List<byte>();

            if (DataAvailable())
            {
                data = ReadPacket();
            }
            else
            {
                data = null;
                return new ReceivedData(data.ToArray());
            }

            if (data[0] == prodByte)
            {
                data.RemoveAt(0);
                return new PacketResponse(data.ToArray());
            }
            else if (data[0] == testByte)
            {
                data.RemoveAt(0);
                return new TestingPacket(data.ToArray());
            }
            else
            {
                data.RemoveAt(0);
                return new ReceivedData(data.ToArray());
            }
        }

        /// <summary>
        /// Sends a packet, consisting of a  header, data, and footer, to the connected device.
        /// </summary>
        /// <param name="data">Byte array of data to send. NB: at this time, packet MUST be 20 bytes.</param>
        /// <param name="success">A boolean value, passed by reference, indicating whether or not the operation was successful.</param>
        /// <param name="errorMessage">A string, passed by reference, containing any error message.</param>
        public void SendPacket(byte[] data, out bool success, out string errorMessage)
        {
            /*Header.CopyTo(packet, 0);
            data.CopyTo(packet, Header.Length);
            Footer.CopyTo(packet, Header.Length + data.Length);
            Send(packet, out success, out errorMessage);*/
            List<byte> packet = new List<byte>();
            packet.AddRange(Header);
            packet.AddRange(data);
            packet.AddRange(Footer);
            Send(packet.ToArray(), out success, out errorMessage);
            return;
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
            while (stream.DataAvailable && i < 100)
            {
                i++;
                data.Add((byte)stream.ReadByte());
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
        /// Reads all data available and returns a string representation. NB: Deprecated in favor of ReadPacket()
        /// </summary>
        /// <returns>Returns a string representation of the bytes read from the stream.</returns>
        private string ReadAllAvailable()
        {
            List<byte> data = new List<byte>();
            while (stream.DataAvailable)
                data.Add((byte)stream.ReadByte());
            return System.Text.Encoding.ASCII.GetString(data.ToArray());
        }

        /// <summary>
        /// Reads a full packet, consisting of a header, data, and a footer, from the stream.
        /// </summary>
        /// <returns>Returns the packet as a byte array.</returns>
        private List<byte> ReadPacket()
        {
            List<byte> data = new List<byte>();
            List<byte> footerList = Footer.ToList<byte>();

            if (Find(Header))
            {
                System.Threading.Thread.Sleep(10);
                while (stream.DataAvailable)
                {
                    data.Add((byte)stream.ReadByte());
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

            return data;
        }

        /// <summary>
        /// Sends a single byte to the connected device.
        /// </summary>
        /// <param name="message">Single byte to send.</param>
        /// <param name="success">A boolean value, passed by reference, indicating whether or not the operation was successful.</param>
        /// <param name="errorMessage">A string, passed by reference, containing any error message.</param>
        private void Send(byte message, out bool success, out string errorMessage)
        {
            byte[] toSend = { message };
            Send(message, out success, out errorMessage);
            return;
        }

        /// <summary>
        /// Sends a byte array to the connected device.
        /// </summary>
        /// <param name="message">Byte array to send.</param>
        /// <param name="success">A boolean value, passed by reference, indicating whether or not the operation was successful.</param>
        /// <param name="errorMessage">A string, passed by reference, containing any error message.</param>
        private void Send(byte[] message, out bool success, out string errorMessage)
        {
            try
            {
                stream.Write(message, 0, message.Length);
                success = true;
                errorMessage = string.Empty;
                return;
            }
            catch (Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
                return;
            }
        }
    }
}
