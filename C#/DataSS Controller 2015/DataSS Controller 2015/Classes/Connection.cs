using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    public class TcpConnection
    {
        public TcpClient Client;
        public StreamWriter Writer;
        public NetworkStream Stream;

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

        public void Send(string message)
        {
            Writer.Write(message);
            Writer.Flush();
        }

        public void Send(byte message)
        {
            byte[] toSend = { message };
            Stream.Write(toSend, 0, 1);
        }

        public void Send(byte[] message)
        {
            Stream.Write(message, 0, message.Length);
        }

        public string ReadAllAvailable()
        {
            List<byte> data = new List<byte>();
            while (Stream.DataAvailable)
                data.Add((byte)Stream.ReadByte());
            return System.Text.Encoding.ASCII.GetString(data.ToArray());
        }
    }
}
