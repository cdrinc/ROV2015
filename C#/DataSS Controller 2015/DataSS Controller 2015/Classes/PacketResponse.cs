using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace DataSS_Controller_2015.Classes
{
    /// <summary>
    /// A generic base class for a packet received from the connected microcontroller.
    /// </summary>
    public class PacketResponse
    {
        private byte[] ReceivedPacket;

        public PacketResponse()
        {
            ReceivedPacket = null;
        }

        public PacketResponse(byte[] data)
        {
            ReceivedPacket = data;
        }

        public override string ToString()
        {
            return System.Text.ASCIIEncoding.ASCII.GetString(ReceivedPacket);
        }
    }
}
