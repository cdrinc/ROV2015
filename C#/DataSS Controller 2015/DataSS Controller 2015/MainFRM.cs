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
            }
            if (gameRadioButton.Checked)
            {
                controller = new Classes.GameController();
                controller.InputChanged += controller_InputChanged;
            }
        }

        void controller_InputChanged(object sender, EventArgs e)
        {
            forwardNum.Value = (decimal)controller.LS.Y;
            translateNum.Value = (decimal)controller.LS.X;
            upDownNum.Value = (decimal)controller.RS.Y;
            yawNum.Value = (decimal)controller.RS.X;

            aNum.Value = controller.A == false ? 0 : 1;
            bNum.Value = controller.B == false ? 0 : 1;
            xNum.Value = controller.X == false ? 0 : 1;
            yNum.Value = controller.Y == false ? 0 : 1;
            rbNum.Value = controller.RB == false ? 0 : 1;
            lbNum.Value = controller.LB == false ? 0 : 1;

            rtNum.Value = (decimal)controller.RT;
            ltNum.Value = (decimal)controller.LT;
        }


    }
}
