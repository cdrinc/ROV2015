using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace DataSS_Controller_2015.Classes
{
    /// <summary>
    /// Contains all methods used to modify transmitted data based on received data.
    /// </summary>
    public class Processor
    {
        private CommandData sendData;
        private Controller controller;
        private enum ControlStates { Manual, Hover, Mediated}
        private int ProcessorState;
        private const int TimerInterval = 20;
        private PacketResponse incoming;
        private ReceivedData sensorData;
        private TcpConnection Connection;
        private Thread PollThread;
        private System.Timers.Timer ModelTimer;

        /// <summary>
        /// A delegate representing a method to be called when the InputChanged event is fired.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        public delegate void ProcessorHandler(object sender, ProcessorEventArgs e);

        /// <summary>
        /// A delegate representing a method to be called when the IncomingData event is fired.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        public delegate void ReceiveHandler(object sender, ProcessorEventArgs e);

        /// <summary>
        /// An event that indicates that the controller's state has changed.
        /// </summary>
        public event ProcessorHandler InputChanged;

        /// <summary>
        /// An event that indicates that the TcpConnection should be read for incoming data.
        /// </summary>
        public event ReceiveHandler IncomingData;

        public CommandData SendData
        {
            get { return this.sendData; }
            set { }
        }

        public Controller Controller
        {
            get { return this.controller; }
            set { }
        }

        public PacketResponse Incoming
        {
            get { return this.incoming; }
            set { }
        }
        
        public ReceivedData SensorData
        {
            get { return this.sensorData; }
            set { this.sensorData = value; }
        }


        public Processor(bool gamePad, TcpConnection connection)
        {
            ProcessorState = (int)ControlStates.Manual;
            this.Connection = connection;
            if (gamePad)
            {
                controller = new GameController();
            }
            else
            {
                controller = new KeyboardController();
            }
        }

        public void Begin()
        {
            PollThread = new Thread(new ThreadStart(Poll));
            PollThread.Start();
        }

        public void End()
        {
            if (PollThread != null)
                if (PollThread.IsAlive)
                    PollThread.Abort();
        }

        public void Poll()
        {
            bool changed;
            // polls controller
            controller.Poll(out changed);
            // gets any sensor data and stores it in sensorData
            GetData();

            // transforms the controller data
            // modification by sensorData yet to be implemented
            this.sendData = Transform(controller, sensorData);

            // if the controller state has changed, reports it to the gui thread and sends the data down the pipe
            if (changed)
            {
                ReportController();
            }
        }

        private void GetData()
        {
            IncomingData(this, new ProcessorEventArgs());
        }

        
        public CommandData Transform(Controller controller, ReceivedData sensorData)
        {
            CommandData data = new CommandData();
            // this sets the Pump motor depending on the state of the A button
            data.Pump = (byte)controller.A;
            #region Length
            // this sets the Length motor to either full on or full off, depending on the state of the DPad
            if (controller.DLeft != 0)
            {
                data.Length = 1;
            }
            else if (controller.DRight != 0)
            {
                data.Length = 2;
            }
            else
            {
                data.Length = 0;
            }
            #endregion

            #region Valve
            // this sets the Valve motor to either full on or full off, depending on the state of the DPad
            if (controller.DUp != 0)
            {
                data.Valve = 1;
            }
            else if (controller.DDown != 0)
            {
                data.Valve = 2;
            }
            else
            {
                data.Valve = 0;
            }
            #endregion

            // these lines set the vertical motors to the Y-value of RS
            data.VerticalB = Utilities.MapStick(controller.RS.Y);
            data.VerticalM = Utilities.MapStick(controller.RS.Y);
            data.VerticalF = Utilities.MapStick(controller.RS.Y);

            // sometimes the Length property is slightly greater than 1
            // if that happens, this rectifies it
            float l = controller.LS.Length();
            if (l > 1)
            {
                l = 1;
            }

            if (Math.Sign(controller.LS.X) == Math.Sign(controller.LS.Y))
            {
                data.TranslateFL = Utilities.MapStick(Math.Sign(controller.LS.Y) * l);
                data.TranslateBR = Utilities.MapStick(Math.Sign(controller.LS.Y) * l);

                data.TranslateFR = Utilities.MapStick(controller.LS.Y - controller.LS.X);
                data.TranslateBL = Utilities.MapStick(controller.LS.Y - controller.LS.X);
            }
            else if (Math.Sign(controller.LS.X) != Math.Sign(controller.LS.Y))
            {
                data.TranslateFL = Utilities.MapStick(controller.LS.Y + controller.LS.X);
                data.TranslateBR = Utilities.MapStick(controller.LS.Y + controller.LS.X);

                data.TranslateFR = Utilities.MapStick(Math.Sign(controller.LS.Y) * l);
                data.TranslateBL = Utilities.MapStick(Math.Sign(controller.LS.Y) * l);
            }
            else if (controller.LS.X == 0)
            {
                data.TranslateFL = Utilities.MapStick(controller.LS.Y);
                data.TranslateFR = Utilities.MapStick(controller.LS.Y);
                data.TranslateBL = Utilities.MapStick(controller.LS.Y);
                data.TranslateBR = Utilities.MapStick(controller.LS.Y);
            }
            else if (controller.LS.Y == 0)
            {
                data.TranslateFL = Utilities.MapStick(controller.LS.X);
                data.TranslateFR = Utilities.MapStick(-1 * controller.LS.X);
                data.TranslateBL = Utilities.MapStick(-1 * controller.LS.X);
                data.TranslateBR = Utilities.MapStick(controller.LS.X);
            }

            return data;
        }

        private void ReportController()
        {
            InputChanged(this, new ProcessorEventArgs());
        }

    }
}
