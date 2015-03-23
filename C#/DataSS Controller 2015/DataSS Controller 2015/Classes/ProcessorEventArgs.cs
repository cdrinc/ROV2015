using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    public class ProcessorEventArgs : EventArgs
    {
        public string Message { get; set; }
        public ReceivedData Data { get; set; }
        public ProcessorEventArgs()
        {

        }

        public ProcessorEventArgs(string message)
        {
            this.Message = message;
        }

        public ProcessorEventArgs(ReceivedData data)
        {
            this.Data = data;
        }
    }
}
