using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DataSS_Controller_2015.Classes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1516:ElementsMustBeSeparatedByBlankLine", Justification = "Reviewed. Suppression is OK here.")]

    /// <summary>
    /// Provides functionality related to obtaining values to send down the TcpConnection.
    /// </summary>
    public class Controller
    {
        // protected fields that hold values for value-type properties (Vector2)
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "For simplicity, it is easier if derived classes can access this field directly.")]
        protected Vector2 ls;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "For simplicity, it is easier if derived classes can access this field directly.")]
        protected Vector2 rs;

        /// <summary>
        /// A delegate representing a method to be called when the InputChanged event is fired.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        public delegate void ControllerHandler(object sender, ControllerEventArgs e);

        /// <summary>
        /// A delegate representing a method to be called when the IncomingData event is fired.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The arguments passed by the event.</param>
        public delegate void ReceiveHandler(object sender, EventArgs e);

        /// <summary>
        /// An event that indicates that the controller's state has changed.
        /// </summary>
        public event ControllerHandler InputChanged;

        /// <summary>
        /// An event that indicates that the TcpConnection should be read for incoming data.
        /// </summary>
        public event ReceiveHandler IncomingData;

        // public properties that are updated with control values as polling is done
        // all integers are either zero or one, false and true respectively
        // all floats are between 0 and 1
        public Vector2 LS 
        {
            get { return ls; }
            set { }
        }

        public Vector2 RS 
        {
            get { return rs; }
            set { }
        }

        public float LT { get; set; }
        public float RT { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int RB { get; set; }
        public int LB { get; set; }
        public int DUp { get; set; }
        public int DLeft { get; set; }
        public int DRight { get; set; }
        public int DDown { get; set; }
        public int LSClick { get; set; }
        public int RSClick { get; set; }
        public int Start { get; set; }
        public int Back { get; set; }

        /// <summary>
        /// Non-implemented polling function (overridden in child classes).
        /// </summary>
        public virtual void BeginPolling()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Non-implemented polling function (overridden in child classes).
        /// </summary>
        public virtual void EndPolling()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Non-implemented polling function (overridden in child classes).
        /// </summary>
        /// <returns>Returns a boolean value indicating whether or not the Polling method is active.</returns>
        public virtual bool IsPolling()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fires the InputChanged event.
        /// </summary>
        protected virtual void OnInputChanged(ControllerData data)
        {
            InputChanged(this, new ControllerEventArgs(data));
        }

        /// <summary>
        /// Fires the IncomingData event.
        /// </summary>
        protected virtual void OnIncomingData()
        {
            IncomingData(this, new EventArgs());
        }
    }
}
