using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    /// <summary>
    /// An argument passed to an InputChanged or IncomingData event.
    /// </summary>
    public class ControllerEventArgs : EventArgs
    {
        private ControllerData data;

        public ControllerData Data
        {
            get 
            { 
                return data; 
            }
            set 
            {
                if (value is ControllerData)
                    data = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerEventArgs"/> class.
        /// </summary>
        public ControllerEventArgs(ControllerData data)
        {
            this.data = data;
        }
    }
}
