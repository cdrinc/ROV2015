using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    public class CommandData
    {
        private byte translateFR;
        private byte translateFL;
        private byte translateBL;
        private byte translateBR;
        private byte verticalF;
        private byte verticalM;
        private byte verticalB;

        public byte Meta
        {
            get { return 0x01; }
            set { }
        }
        public byte TranslateFL
        {
            get { return translateFL; }
            set
            {
                if (value != 125)
                {
                    translateFL = value;
                }
                else
                {
                    translateFL = 126;
                }
            }
        }

        public byte TranslateFR
        {
            get { return translateFR; }
            set
            {
                if (value != 125)
                {
                    translateFR = value;
                }
                else
                {
                    translateFR = 126;
                }
            }
        }

        public byte TranslateBL
        {
            get { return translateBL; }
            set
            {
                if (value != 125)
                {
                    translateBL = value;
                }
                else
                {
                    translateBL = 126;
                }
            }
        }

        public byte TranslateBR
        {
            get { return translateBR; }
            set
            {
                if (value != 125)
                {
                    translateBR = value;
                }
                else
                {
                    translateBR = 126;
                }
            }
        }

        public byte VerticalF
        {
            get { return verticalF; }
            set
            {
                if (value != 125)
                {
                    verticalF = value;
                }
                else
                {
                    verticalF = 126;
                }
            }
        }

        public byte VerticalM
        {
            get { return verticalM; }
            set
            {
                if (value != 125)
                {
                    verticalM = value;
                }
                else
                {
                    verticalM = 126;
                }
            }
        }

        public byte VerticalB
        {
            get { return verticalB; }
            set
            {
                if (value != 125)
                {
                    verticalB = value;
                }
                else
                {
                    verticalB = 126;
                }
            }
        }

        public byte Pump { get; set; }
        public byte Valve { get; set; }
        public byte Length { get; set; }
        public byte Hand { get; set; }
        public byte Hover { get; set; }

        /// <summary>
        /// Converts the data stored in the class to a byte array in the correct format for transmission.
        /// </summary>
        /// <returns>A byte array containing the data to be sent.</returns>
        public byte[] Serialize()
        {
            List<byte> byteList = new List<byte>() { Meta, translateFL, translateFR, translateBL, translateBR, verticalF, verticalB, Pump, Valve, Length, Hand };
            byte[] byteArr = byteList.ToArray();
            return byteArr;
        }
    }
}
