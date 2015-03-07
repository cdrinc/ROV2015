using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

using DataSS_Controller_2015.Classes;

namespace DataSS_Controller_2015
{
    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainFRM : Form
    {
        private TcpConnection connection;
        //private Controller controller;
        private Processor DataProcessor;
        private bool connected = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFRM"/> class.
        /// </summary>
        public MainFRM()
        {
            InitializeComponent();
            disconnectButton.Enabled = false;
            IPcBox.DataSource = GetAddresses();
            portcBox.DataSource = GetPorts();
        }

        #region Main Form Events
        /// <summary>
        /// Executes on loading of the MainFRM object.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        private void MainFRM_Load(object sender, EventArgs e)
        {
            InitializeController(gameRadioButton.Checked);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        
        /// <summary>
        /// Attempts to initialize a connection with a connected device.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        private void connectButton_Click(object sender, EventArgs e)
        {
            ethernetListenListBox.Items.Add("Initializing Connection with " + IPcBox.Text + "...");
            connection = new TcpConnection(IPcBox.Text, int.Parse(portcBox.Text));

            bool success;
            string message;
            connection.Connect(out success, out message);
            if (!success)
            {
                ethernetListenListBox.AddToEnd(message);
                return;
            }

            connection.Handshake(out success, out message);
            if (!success)
            {
                connection.Close();
                ethernetListenListBox.AddToEnd(message);
                return;
            }

            connectButton.Enabled = false;
            disconnectButton.Enabled = true;
            connected = true;
            ethernetListenListBox.AddToEnd("Connected!");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]

        /// <summary>
        /// Attempts to disconnect from a connected device.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        private void disconnectButton_Click(object sender, EventArgs e)
        {
            EndConnection();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]

        /// <summary>
        /// Calls the InitializeController method with either a keyboard or gamepad, depending on the state of the form's radio button.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        private void controllerStartButton_Click(object sender, EventArgs e)
        {
            InitializeController(gameRadioButton.Checked);
        }

        /// <summary>
        /// Terminates the thread that is polling the controller.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        private void MainFRM_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DataProcessor != null)
                DataProcessor.End();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]

        /// <summary>
        /// Exits the application.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        /// <summary>
        /// Closes the connection and enables and disables the appropriate buttons.
        /// </summary>
        private void EndConnection()
        {
            connection.Close();
            connected = false;
            disconnectButton.Enabled = false;
            connectButton.Enabled = true;
        }

        /// <summary>
        /// Initializes the controller (gamepad or keyboard).
        /// </summary>
        /// <param name="isGamepad">Boolean value indicating whether or not the controller should be a gamepad.</param>
        private void InitializeController(bool isGamepad)
        {
            if (isGamepad)
            {
                //controller = new Classes.GameController();
                //controller.InputChanged += new Controller.ControllerHandler(controller_InputChanged);
                //controller.IncomingData += new Controller.ReceiveHandler(controller_IncomingData);
                //((GameController)controller).BeginPolling();
                controllerStartButton.Text = "Gamepad";
                DataProcessor = new Processor(true, connection);
            }
            else
            {
                //controller = new Classes.KeyboardController();
                //controller.InputChanged += new Controller.ControllerHandler(controller_InputChanged);
                //((KeyboardController)controller).BeginPolling();
                controllerStartButton.Text = "Keyboard";
                DataProcessor = new Processor(false, connection);
            }
            
            DataProcessor.InputChanged += new Processor.ProcessorHandler(processor_InputChanged);
            DataProcessor.IncomingData += new Processor.ReceiveHandler(processor_IncomingData);
            DataProcessor.Begin();
            controllerStartButton.Enabled = false;
        }

        /// <summary>
        /// Composes a list of IP addresses predetermined to be valid.
        /// </summary>
        /// <returns>Returns a predetermined list of IP address strings.</returns>
        private List<string> GetAddresses()
        {
            List<string> ips = new List<string>();
            ips.Add("169.254.60.110"); // works on mac 1st port
            ips.Add("169.254.180.60"); // works on mac 2nd port
            ips.Add("192.168.137.2"); // works on pc
            return ips;
        }

        /// <summary>
        /// Composes a list of ports predetermined to be valid.
        /// </summary>
        /// <returns>Returns a predetermined list of port integers.</returns>
        private List<int> GetPorts()
        {
            List<int> ports = new List<int>();
            ports.Add(13000);
            return ports;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]

        /// <summary>
        /// Adds incoming data to the form's listbox.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        private void processor_IncomingData(object sender, ProcessorEventArgs e)
        {
            this.Invoke((Action)delegate
            {
                ReceivedData data;

                if (connection != null)
                {
                    if (connection.Connected && connection.DataAvailable())
                    {
                        data = connection.GetResponse();

                        if (data is PacketResponse || data is TestingPacket)
                        {
                            dataListenBox.AddToEnd(data.ToString());
                        }
                        else
                        {
                            ethernetListenListBox.AddToEnd(data.ToString());
                        }

                        DataProcessor.SensorData = data;
                    }
                }
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]

        /// <summary>
        /// Attempts to send the new controller state across the connection.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        private void processor_InputChanged(object sender, ProcessorEventArgs e)
        {
            this.Invoke((Action)delegate
            {
                forwardNum.Value = (decimal)DataProcessor.Controller.LS.Y * 100;
                translateNum.Value = (decimal)DataProcessor.Controller.LS.X * 100;
                upDownNum.Value = (decimal)DataProcessor.Controller.RS.Y * 100;
                yawNum.Value = (decimal)DataProcessor.Controller.RS.X * 100;

                aNum.Value = DataProcessor.Controller.A;
                bNum.Value = DataProcessor.Controller.B;
                xNum.Value = DataProcessor.Controller.X;
                yNum.Value = DataProcessor.Controller.Y;

                rbNum.Value = DataProcessor.Controller.RB;
                lbNum.Value = DataProcessor.Controller.LB;
                rtNum.Value = (decimal)DataProcessor.Controller.RT * 100;
                ltNum.Value = (decimal)DataProcessor.Controller.LT * 100;

                rsNum.Value = DataProcessor.Controller.RSClick;
                lsNum.Value = DataProcessor.Controller.LSClick;

                startNum.Value = DataProcessor.Controller.Start;
                backNum.Value = DataProcessor.Controller.Back;

                upNum.Value = DataProcessor.Controller.DUp;
                leftNum.Value = DataProcessor.Controller.DLeft;
                rightNum.Value = DataProcessor.Controller.DRight;
                downNum.Value = DataProcessor.Controller.DDown;

                if (connected)
                {
                    /*RawSentData sending = new RawSentData();
                    sending.A = (byte)controller.A;
                    sending.B = (byte)controller.B;
                    sending.X = (byte)controller.X;
                    sending.Y = (byte)controller.Y;
                    sending.RB = (byte)controller.RB;
                    sending.LB = (byte)controller.LB;
                    sending.RT = (byte)Utilities.Map(controller.RT, 0, 1, 0, 255);
                    sending.LT = (byte)Utilities.Map(controller.LT, 0, 1, 0, 255);
                    sending.LSY = (byte)Utilities.MapStick(controller.LS.Y);
                    sending.LSX = (byte)Utilities.MapStick(controller.LS.X);
                    sending.RSY = (byte)Utilities.MapStick(controller.RS.Y);
                    sending.RSX = (byte)Utilities.MapStick(controller.RS.X);
                    sending.DUp = (byte)controller.DUp;
                    sending.DDown = (byte)controller.DDown;
                    sending.DLeft = (byte)controller.DLeft;
                    sending.DRight = (byte)controller.DRight;
                    sending.LSClick = (byte)controller.LSClick;
                    sending.RSClick = (byte)controller.RSClick;
                    sending.Start = (byte)controller.Start;
                    sending.Back = (byte)controller.Back;*/

                    CommandData sending = DataProcessor.SendData;

                    bool success;
                    string errorMessage;
                    byte[] sendData = sending.Serialize();
                    connection.SendPacket(sendData, out success, out errorMessage);

                    if (!success)
                    {
                        ethernetListenListBox.AddToEnd(errorMessage);
                        EndConnection();
                    }
                }
            });
        }

        private void errButton_Click(object sender, EventArgs e)
        {
            List<byte> sending = new List<byte>();
            sending.Add(0x02);
            sending.AddRange(System.Text.Encoding.ASCII.GetBytes("RST"));
            bool success;
            string message;
            connection.SendPacket(sending.ToArray(), out success, out message);
        }
    }
}
