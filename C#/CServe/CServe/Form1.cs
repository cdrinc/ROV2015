using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace CServe
{
    public partial class Form1 : Form
    {

        //byte[] data;
        //UdpClient client;
        IPEndPoint localEp;
        //IPAddress receiverIp;
        //IPEndPoint receiverEp;
        //BackgroundWorker worker = new BackgroundWorker();

        TcpListener server = null; //creates the tcp server
        int tcpPort = 13000;
        public Form1()
        {
            InitializeComponent();
            localEp = new IPEndPoint(IPAddress.Any, tcpPort);
            //receiverIp = IPAddress.Parse("169.254.60.110");
            //receiverEp = new IPEndPoint(receiverIp, 8888);
            //client = new UdpClient();
            server = new TcpListener(localEp);
            server.Start();
            MessageBox.Show("Listening for connections!");
        }

        private void startListening_Click(object sender, EventArgs e)
        {
            //UdpClient udpClient = new UdpClient("169.254.60.110", 8888);
            //Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there");
            //try
            //{
            //    udpClient.Send(sendBytes, sendBytes.Length);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
            //Console.ReadLine();
            try
            {
                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop. 
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests. 
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client. 
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        private void sendData_click(object sender, EventArgs e)
        {

        }
    }
}
