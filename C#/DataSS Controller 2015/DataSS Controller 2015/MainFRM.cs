using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DataSS_Controller_2015.Classes;

namespace DataSS_Controller_2015
{
    public partial class MainFRM : Form
    {
        TcpConnection connection;
        Controller controller;

        public MainFRM()
        {
            InitializeComponent();
            button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connection = new TcpConnection("169.254.60.110", 13000);
            button2.Enabled = true;
        }

        private void MainFRM_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            connection.Send(textBox1.Text);
            listBox1.Items.Add(connection.ReadAllAvailable());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            connection.Close();
        }

        private void controllerStartButton_Click(object sender, EventArgs e)
        {
            /*
            public Vector2 LS = Vector2.Zero;
            public Vector2 RS = Vector2.Zero;
            public float LT = 0;
            public float RT = 0;
            public bool A = false;
            public bool B = false;
            public bool X = false;
            public bool Y = false;
            public bool RB = false;
            public bool LB = false;
            */
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

        void controller_InputChanged(object sender, EventArgs e)
        {
            forwardNum.Invoke((Action)delegate { forwardNum.Value = (decimal)controller.LS.Y; });
            translateNum.Invoke((Action)delegate { translateNum.Value = (decimal)controller.LS.X; });
            upDownNum.Invoke((Action)delegate { upDownNum.Value = (decimal)controller.RS.Y; });
            yawNum.Invoke((Action)delegate { yawNum.Value = (decimal)controller.RS.X; });

            aNum.Invoke((Action)delegate { aNum.Value = controller.A == false ? 0 : 1; });
            bNum.Invoke((Action)delegate { bNum.Value = controller.B == false ? 0 : 1; });
            xNum.Invoke((Action)delegate { xNum.Value = controller.X == false ? 0 : 1; });
            yNum.Invoke((Action)delegate { yNum.Value = controller.Y == false ? 0 : 1; });
            rbNum.Invoke((Action)delegate { rbNum.Value = controller.RB == false ? 0 : 1; });
            lbNum.Invoke((Action)delegate { lbNum.Value = controller.LB == false ? 0 : 1; });

            rtNum.Invoke((Action)delegate { rtNum.Value = (decimal)controller.RT; });
            ltNum.Invoke((Action)delegate { ltNum.Value = (decimal)controller.LT; });
        }


    }
}
