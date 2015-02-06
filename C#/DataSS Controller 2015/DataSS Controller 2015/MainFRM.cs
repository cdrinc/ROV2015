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
            forwardNum.Invoke((Action)delegate { forwardNum.Value = (decimal)controller.LS.Y*100; });
            translateNum.Invoke((Action)delegate { translateNum.Value = (decimal)controller.LS.X*100; });
            upDownNum.Invoke((Action)delegate { upDownNum.Value = (decimal)controller.RS.Y*100; });
            yawNum.Invoke((Action)delegate { yawNum.Value = (decimal)controller.RS.X*100; });

            aNum.Invoke((Action)delegate { aNum.Value = controller.A; });
            bNum.Invoke((Action)delegate { bNum.Value = controller.B; });
            xNum.Invoke((Action)delegate { xNum.Value = controller.X; });
            yNum.Invoke((Action)delegate { yNum.Value = controller.Y; });
            rbNum.Invoke((Action)delegate { rbNum.Value = controller.RB; });
            lbNum.Invoke((Action)delegate { lbNum.Value = controller.LB; });

            rtNum.Invoke((Action)delegate { rtNum.Value = (decimal)controller.RT; });
            ltNum.Invoke((Action)delegate { ltNum.Value = (decimal)controller.LT; });

            rsNum.Invoke((Action)delegate { rsNum.Value = controller.RSClick; });
            lsNum.Invoke((Action)delegate { lsNum.Value = controller.LSClick; });

            startNum.Invoke((Action)delegate { startNum.Value = controller.Start; });
            backNum.Invoke((Action)delegate { backNum.Value = controller.Back; });
            bigNum.Invoke((Action)delegate { bigNum.Value = controller.BigButton; });

            upNum.Invoke((Action)delegate { upNum.Value = controller.DUp; });
            leftNum.Invoke((Action)delegate { leftNum.Value = controller.DLeft; });
            rightNum.Invoke((Action)delegate { rightNum.Value = controller.DRight; });
            downNum.Invoke((Action)delegate { downNum.Value = controller.DDown; });

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
    }
}
