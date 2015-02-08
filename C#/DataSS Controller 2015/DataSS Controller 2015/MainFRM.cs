using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Web.Script.Serialization;

using DataSS_Controller_2015.Classes;

namespace DataSS_Controller_2015
{
    public partial class MainFRM : Form
    {
        TcpConnection connection;
        Controller controller;
        bool connected = false;

        public MainFRM()
        {
            InitializeComponent();
            button2.Enabled = false;
            IPcBox.DataSource = GetAddresses();
            portcBox.DataSource = GetPorts();
        }

        #region Main Form Events
        private void MainFRM_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            connection = new TcpConnection(IPcBox.Text, Int32.Parse(portcBox.Text));
            button2.Enabled = true;
            connected = true;
            connection.Send("connected");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            connection.Send(textBox1.Text);
            listBox1.Items.Add(connection.ReadAllAvailable());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            connection.Close();
            connected = false;
        }

        private void controllerStartButton_Click(object sender, EventArgs e)
        {
            if (keyboardRadioButton.Checked)
            {
                controller = new Classes.KeyboardController();
                controller.InputChanged += new Controller.ControllerHandler(controller_InputChanged);
                ((KeyboardController)controller).BeginPolling();
            }
            if (gameRadioButton.Checked)
            {
                controller = new Classes.GameController();
                controller.InputChanged += new Controller.ControllerHandler(controller_InputChanged);
                ((GameController)controller).BeginPolling();
            }
        }

        private void MainFRM_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (controller != null)
                if (controller is GameController)
                    ((GameController)controller).EndPolling();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        private List<String> GetAddresses()
        {
            List<string> ips = new List<string>();
            ips.Add("169.254.60.110"); //works on mac
            ips.Add("192.168.137.2");
            return ips;
        }

        private List<int> GetPorts()
        {
            List<int> ports = new List<int>();
            ports.Add(13000);
            return ports;
        }

        void controller_InputChanged(object sender, EventArgs e)
        {
            forwardNum.Invoke((Action)delegate { forwardNum.Value = (decimal)controller.LS.Y * 100; });
            translateNum.Invoke((Action)delegate { translateNum.Value = (decimal)controller.LS.X*100; });
            upDownNum.Invoke((Action)delegate { upDownNum.Value = (decimal)controller.RS.Y*100; });
            yawNum.Invoke((Action)delegate { yawNum.Value = (decimal)controller.RS.X*100; });

            aNum.Invoke((Action)delegate { aNum.Value = controller.A; });
            bNum.Invoke((Action)delegate { bNum.Value = controller.B; });
            xNum.Invoke((Action)delegate { xNum.Value = controller.X; });
            yNum.Invoke((Action)delegate { yNum.Value = controller.Y; });
            rbNum.Invoke((Action)delegate { rbNum.Value = controller.RB; });
            lbNum.Invoke((Action)delegate { lbNum.Value = controller.LB; });

            rtNum.Invoke((Action)delegate { rtNum.Value = (decimal)controller.RT*100; });
            ltNum.Invoke((Action)delegate { ltNum.Value = (decimal)controller.LT*100; });

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
                    sending.RT = (byte)Processing.Map(controller.RT, 0, 1, 0, 255);
                    sending.LT = (byte)Processing.Map(controller.LT, 0, 1, 0, 255);
                    sending.LSY = (byte)Processing.Map(controller.LS.Y, 0, 1, 0, 255);
                    sending.LSX = (byte)Processing.Map(controller.LS.X, 0, 1, 0, 255);
                    sending.RSY = (byte)Processing.Map(controller.RS.Y, 0, 1, 0, 255);
                    sending.RSX = (byte)Processing.Map(controller.RS.X, 0, 1, 0, 255);
                    sending.DUp = (byte)controller.DUp;
                    sending.DDown = (byte)controller.DDown;
                    sending.DLeft = (byte)controller.DLeft;
                    sending.DRight = (byte)controller.DRight;
                    sending.LSClick = (byte)controller.LSClick;
                    sending.RSClick = (byte)controller.RSClick;
                    sending.Start = (byte)controller.Start;
                    sending.Back = (byte)controller.Back;

                    JavaScriptSerializer jsonSer = new JavaScriptSerializer();
                    string obj = jsonSer.Serialize(sending);

                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(connection.Stream, obj);
                }
            });

        }
    }
}
