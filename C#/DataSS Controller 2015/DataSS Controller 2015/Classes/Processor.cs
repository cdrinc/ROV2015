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

            data.Hover = (byte)controller.Start;

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

        #region PID Controller
        private double pSetpoint = 0;
        private double pPV = 0;  // actual possition (Process Value)
        private double pError = 0;   // how much SP and PV are diff (SP - PV)
        private double pIntegral = 0; // curIntegral + (error * Delta Time)
        private double pDerivative = 0;  //(error - prev error) / Delta time
        private double pPreError = 0; // error from last time (previous Error)
        private double pKp = 0.2, pKi = 0.01, pKd = 1; // PID constant multipliers
        private double pDt = 100.0; // delta time
        private double pOutput = 0; // the drive amount that effects the PV.
        private double pNoisePercent = 0.02; // amount of the full range to randomly alter the PV
        private double pNoise = 0;  // random noise added to PV

        public double setpoint  //SP, the value desired from the process (i.e. desired temperature of a heater.)
        {
            get { return pSetpoint; }
            set
            {
                pSetpoint = value;
                //// change the label for setpoint value
                //lblSP.Text = pSetpoint.ToString();
                //// change the slider possition if it does not match
                //if ((int)Math.Round(pSetpoint) != trackBarSP.Value) // don't use doubles in == or != expressions due to bit resolution
                //    trackBarSP.Value = (int)Math.Round(pSetpoint);
            }
        }
        public double PV    //Process Value, the measured value of the process (i.e. actual temperature of a heater)
        {
            get { return pPV; }
            set
            {
                pPV = value;
                // place limits on the measured value
                if (pPV < 0)
                    pPV = 0;
                else if (pPV > 1000)
                    pPV = 1000;
                //// update the text
                //lblPV.Text = pPV.ToString();
                //// update progress bar
                //if (pPV > progBarPV.Maximum)
                //    progBarPV.Value = progBarPV.Maximum;
                //else if (pPV < progBarPV.Minimum)
                //    progBarPV.Value = progBarPV.Minimum;
                //else
                //    progBarPV.Value = (int)pPV;

            }
        }
        public double error //difference between Setpoint and Process value (SP - PV)
        {
            get { return pError; }
            set
            {
                pError = value;
                // uodate the lable
                //lblError.Text = pError.ToString();
            }
        }
        public double integral //sum of recent errors
        {
            get { return pIntegral; }
            set
            {
                pIntegral = value;
                // update the lable
                //lblIntegral.Text = integral.ToString();
            }
        }
        public double derivative    //How much the error is changing (the slope of the change)
        {
            get { return pDerivative; }
            set
            {
                pDerivative = value;
                //lblDeriv.Text = derivative.ToString();
            }
        }
        public double preError  //Previous error, the error last time the process was checked.
        {
            get { return pPreError; }
            set { pPreError = value; }
        }
        public double Kp    //proportional gain, a "constant" the error is multiplied by. Partly contributes to the output as (Kp * error)
        {
            get { return pKp; }
            set
            {
                pKp = value;
                //// update the textBox
                //if (pKp.CompareTo(validateDouble(tbKp.Text, pKp)) != 0)
                //    tbKp.Text = pKp.ToString();

            }
        }
        public double Ki    // integral gain, a "constant" the sum of errors will be multiplied by.
        {
            get { return pKi; }
            set
            {
                pKi = value;
                //// update the textBox
                //if (pKi.CompareTo(validateDouble(tbKi.Text, pKi)) != 0)
                //    tbKi.Text = pKi.ToString();

            }
        }
        public double Kd    // derivative gain, a "constant" the rate of change will be multiplied by.
        {
            get { return pKd; }
            set
            {
                pKd = value;
                //// update the textBox
                //if (pKd.CompareTo(validateDouble(tbKd.Text, pKd)) != 0)
                //    tbKd.Text = pKd.ToString();
            }
        }
        public double Dt    // delta time, the interval between saples (in milliseconds).
        {
            get { return pDt; }
            set { pDt = value; }
        }
        public double output    //the output of the process, the value driving the system/equipment.  (i.e. the amount of electricity supplied to a heater.)
        {
            get { return pOutput; }
            set
            {
                pOutput = value;
                // limit the output
                if (pOutput < 0)
                    pOutput = 0;
                else if (pOutput > 1000)
                    pOutput = 1000;

                //if (pOutput > progBarOut.Maximum)
                //    progBarOut.Value = progBarOut.Maximum;
                //else if (pOutput < progBarOut.Minimum)
                //    progBarOut.Value = progBarOut.Minimum;
                //else
                //    progBarOut.Value = (int)pOutput;

                //lblOutput.Text = pOutput.ToString();
            }
        }
        public double noisePercent  //upper limit to the amount of artificial noise (random distortion) to add to the PV (measured value).  0.0 to 1.0 (0 to 100%) 
        {
            get { return pNoisePercent; }
            set { pNoisePercent = value; }
        }

        public double noise     //amount of random noise added to the process value
        {
            get { return pNoise; }
            set { pNoise = value; }
        }

        //fast
        private void ProcessValueUpdate(object sender, EventArgs e)
        {
            /* this my version of cruise control. 
             * PV = PV + (output * .2) - (PV*.10);
             * The equation contains values for speed, efficiency,
             *  and wind resistance.
               Here 'PV' is the speed of the car.
               'output' is the amount of gas supplied to the engine.
             * (It is only 20% efficient in this example)
               And it looses energy at 10% of the speed. (The faster the 
               car moves the more PV will be reduced.)
             * Noise is added randomly if checked, otherwise noise is 0.0
             * (Noise doesn't relate to the cruise control, but can be useful
             *  when modeling other processes.)
             */
            PV = PV + (output * 0.20) - (PV * 0.10) + noise;
            // change the above equation to fit your aplication
        }

        //slow, real updating
        private void ActualUpdating()
        {   /*
             * Pseudocode from Wikipedia
             * 
                previous_error = 0
                integral = 0 
            start:
                error = setpoint - PV(actual_position)
                integral = integral + error*dt
                derivative = (error - previous_error)/dt
                output = Kp*error + Ki*integral + Kd*derivative
                previous_error = error
                wait(dt)
                goto start
             */
            // calculate the difference between the desired value and the actual value
            error = setpoint - PV;
            // track error over time, scaled to the timer interval
            integral = integral + (error * Dt);
            // determin the amount of change from the last time checked
            derivative = (error - preError) / Dt;

            // calculate how much drive the output in order to get to the 
            // desired setpoint. 
            output = (Kp * error) + (Ki * integral) + (Kd * derivative);

            // remember the error for the next time around.
            preError = error;

        }
        #endregion
    }
}