using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    public class CommandData
    {
        public byte Meta
        {
            get { return 0x00; }
            set { }
        }
        public byte TranslateFL { get; set; }
        public byte TranslateFR { get; set; }
        public byte TranslateBL { get; set; }
        public byte TranslateBR { get; set; }
        public byte VerticalF { get; set; }
        public byte VerticalM { get; set; }
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
            List<byte> byteList = new List<byte>() { Meta, TranslateFL, TranslateFR, TranslateBL, TranslateBR, VerticalF, VerticalM, VerticalB, Pump, Valve, Length };
            byte[] byteArr = byteList.ToArray();
            return byteArr;
        }
    }
}
