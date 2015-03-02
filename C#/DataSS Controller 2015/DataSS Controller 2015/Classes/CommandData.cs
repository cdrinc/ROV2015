using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    class CommandData
    {
        public byte Meta
        {
            get { return 0x00; }
            set { }
        }
        public byte ForwardFL { get; set; }
        public byte ForwardFR { get; set; }
        public byte ForwardBL { get; set; }
        public byte ForwardBR { get; set; }
        public byte StrafeF { get; set; }
        public byte StrafeB { get; set; }
        public byte VerticalF { get; set; }
        public byte VerticalB { get; set; }
        public byte Pump { get; set; }
        public byte Valve { get; set; }
        public byte Length { get; set; }

        /// <summary>
        /// Converts the data stored in the class to a byte array in the correct format for transmission.
        /// </summary>
        /// <returns>A byte array containing the data to be sent.</returns>
        public byte[] Serialize()
        {
            List<byte> byteList = new List<byte>() { Meta, ForwardFL, ForwardFR, ForwardBL, ForwardBR, StrafeF, StrafeB, VerticalF, VerticalB, Pump, Valve, Length };
            byte[] byteArr = byteList.ToArray();
            return byteArr;
        }
    }
}
