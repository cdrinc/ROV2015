using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataSS_Controller_2015.Classes
{
    /// <summary>
    /// Contains all methods used to modify transmitted data based on received data.
    /// </summary>
    public class Processor
    {
        private CommandData sending;
        private Controller controller;
        private PacketResponse incoming;
        private TcpConnection Connection;
        private Thread PollThread;

        /// <summary>
        /// A delegate representing a method to be called when the InputChanged event is fired.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        public delegate void ProcessorHandler(object sender, ControllerEventArgs e);

        /// <summary>
        /// A delegate representing a method to be called when the IncomingData event is fired.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        public delegate void ReceiveHandler(object sender, ControllerEventArgs e);

        /// <summary>
        /// An event that indicates that the controller's state has changed.
        /// </summary>
        public event ProcessorHandler InputChanged;

        /// <summary>
        /// An event that indicates that the TcpConnection should be read for incoming data.
        /// </summary>
        public event ReceiveHandler IncomingData;

        private delegate ReceivedData Recieve();
        private Recieve ReadData;

        public CommandData Sending
        {
            get { return this.sending; }
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

        public Processor(bool gamePad, TcpConnection connection)
        {
            this.Connection = connection;
            ReadData += Connection.GetResponse;
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

        }

        public void End()
        {

        }

        public void Poll()
        {
            controller.Poll();
            ReceivedData sensorData = (ReceivedData)MainFRM.ActiveForm.Invoke(ReadData);

        }

        public CommandData Transform(Controller controller, ReceivedData sensorData)
        {

        }

        public void Update()
        {
            InputChanged(this, new ControllerEventArgs());
        }

    }
}
