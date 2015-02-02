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
        public TcpClient client;
        public StreamWriter writer;
        public NetworkStream stream;

        public TcpConnection(string ipAddress, int port)
        {
            try
            {
                // Create a TcpClient. 
                // Note, for this client to work you need to have a TcpServer  
                // connected to the same address as specified by the server, port 
                // combination.
                client = new TcpClient();
                client.Connect(ipAddress, port);

                // Get a client stream for reading and writing. 
                // Stream stream = client.GetStream();
                stream = client.GetStream();

                //create a writer for the network stream
                writer = new StreamWriter(stream);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("It's fucked.\n\n"+ex.Message, "Fucked it is.", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public void Close()
        {
            writer.Close();
            client.Close();
        }

        public void Send(string message)
        {
            writer.Write(message);
            writer.Flush();
        }

        public string ReadAllAvailable()
        {
            List<byte> data = new List<byte>();
            while (stream.DataAvailable)
                data.Add((byte)stream.ReadByte());
            return System.Text.Encoding.ASCII.GetString(data.ToArray());
        }
    }
}
