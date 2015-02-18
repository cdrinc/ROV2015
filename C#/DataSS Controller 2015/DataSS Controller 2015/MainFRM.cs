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
using System.Windows.Threading;

using DataSS_Controller_2015.Classes;

namespace DataSS_Controller_2015
{
    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainFRM : Form
    {
        private TcpConnection connection;
        private Controller controller;
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
            if (controller != null)
                if (controller is GameController)
                    ((GameController)controller).EndPolling();
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
                controller = new Classes.GameController();
                controller.InputChanged += new Controller.ControllerHandler(controller_InputChanged);
                controller.IncomingData += new Controller.ReceiveHandler(controller_IncomingData);
                ((GameController)controller).BeginPolling();
                controllerStartButton.Text = "Gamepad";
            }
            else
            {
                controller = new Classes.KeyboardController();
                controller.InputChanged += new Controller.ControllerHandler(controller_InputChanged);
                ((KeyboardController)controller).BeginPolling();
                controllerStartButton.Text = "Keyboard";
            }

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
        private void controller_IncomingData(object sender, EventArgs e)
        {
            this.Invoke((Action)delegate
            {
                PacketResponse data;
                if (connection != null)
                {
                    if (connection.Connected && connection.DataAvailable())
                    {
                        data = connection.GetResponse();
                        ethernetListenListBox.AddToEnd(data.ToString());
                    }
                }

                return;
            });
        }

        // will be used when switched to wpf
        public void OnIncomingData(object sender, ControllerEventArgs e)
        {
            ControllerData data = e.Data;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]

        /// <summary>
        /// Attempts to send the new controller state across the connection.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        private void controller_InputChanged(object sender, EventArgs e)
        {
            forwardNum.Invoke((Action)delegate { forwardNum.Value = (decimal)controller.LS.Y * 100; });
            translateNum.Invoke((Action)delegate { translateNum.Value = (decimal)controller.LS.X * 100; });
            upDownNum.Invoke((Action)delegate { upDownNum.Value = (decimal)controller.RS.Y * 100; });
            yawNum.Invoke((Action)delegate { yawNum.Value = (decimal)controller.RS.X * 100; });

            aNum.Invoke((Action)delegate { aNum.Value = controller.A; });
            bNum.Invoke((Action)delegate { bNum.Value = controller.B; });
            xNum.Invoke((Action)delegate { xNum.Value = controller.X; });
            yNum.Invoke((Action)delegate { yNum.Value = controller.Y; });
            rbNum.Invoke((Action)delegate { rbNum.Value = controller.RB; });
            lbNum.Invoke((Action)delegate { lbNum.Value = controller.LB; });

            rtNum.Invoke((Action)delegate { rtNum.Value = (decimal)controller.RT * 100; });
            ltNum.Invoke((Action)delegate { ltNum.Value = (decimal)controller.LT * 100; });

            rsNum.Invoke((Action)delegate { rsNum.Value = controller.RSClick; });
            lsNum.Invoke((Action)delegate { lsNum.Value = controller.LSClick; });

            startNum.Invoke((Action)delegate { startNum.Value = controller.Start; });
            backNum.Invoke((Action)delegate { backNum.Value = controller.Back; });

            upNum.Invoke((Action)delegate { upNum.Value = controller.DUp; });
            leftNum.Invoke((Action)delegate { leftNum.Value = controller.DLeft; });
            rightNum.Invoke((Action)delegate { rightNum.Value = controller.DRight; });
            downNum.Invoke((Action)delegate { downNum.Value = controller.DDown; });

            this.Invoke((Action)delegate
            {
                if (connected)
                {
                    SentData sending = new SentData();
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
                    sending.Back = (byte)controller.Back;

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
    }
}
