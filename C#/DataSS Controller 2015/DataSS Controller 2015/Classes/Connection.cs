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

        private byte[] stx = { 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B, 0x7B };
        public byte[] STX
        {
            get { return stx; }
            set { }
        }
        public byte[] etx = { 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D, 0x7D };
        public byte[] ETX
        {
            get { return etx; }
            set { }
        }

        private byte[] blankData = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
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

        public void Handshake()
        {
            SendPacket(blankData);
        }

        public bool DataAvailable()
        {
            return Stream.DataAvailable;
        }

        public void Send(string message)
        {
            Writer.Write(message);
            Writer.Flush();
        }

        private void Send(byte message)
        {
            byte[] toSend = { message };
            Stream.Write(toSend, 0, 1);
        }

        private void Send(byte[] message)
        {
            Stream.Write(message, 0, message.Length);
        }

        public void SendPacket(byte[] data)
        {
            STX.CopyTo(packet, 0);
            data.CopyTo(packet, STX.Length);
            ETX.CopyTo(packet, STX.Length + data.Length);
            Send(packet);
        }

        public string ReadAllAvailable()
        {
            List<byte> data = new List<byte>();
            while (Stream.DataAvailable)
                data.Add((byte)Stream.ReadByte());
            return System.Text.Encoding.ASCII.GetString(data.ToArray());
        }

        public string ReadPacket()
        {
            int i = 0;
            List<byte> data = new List<byte>();
            List<byte> footerList = ETX.ToList<byte>();

            if (Find(STX))
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
            //tried to use regex
            //string regexData = UnescapeString(parsedData);
            //if (regexData != "")
            //{
            //    return regexData;
            //}
            return parsedData;
        }

        private string UnescapeString(string str)
        {
            string unescape = "";
            string pattern = @"\d+";
            Regex r = new Regex(pattern);
            Match m = r.Match(str);
            if (m.Success)
            {
                unescape = m.Value;
            }
            return unescape;
        }
    }
}
