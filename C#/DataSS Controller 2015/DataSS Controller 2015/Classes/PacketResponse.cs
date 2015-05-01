using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    /// <summary>
    /// An object that corresponds to a standard, data-containing packet response from the microcontroller.
    /// </summary>
    public class PacketResponse : ReceivedData
    {

        public float Depth;
        public float Voltage;
        public float Length;

        private float depthConvFactor = (float)1.0197162;

        private List<float> floats = new List<float>();

        public PacketResponse(byte[] data)
        {

            if (data.Length != 12)
            {
                return;
            }
            else
            {
                byte[] b = new byte[10];
                for (int i = 0; i < 3; i++)
                {
                    floats[i] = System.BitConverter.ToSingle(data, i * 4);
                }

                Depth = floats[0] * depthConvFactor;
                Voltage = floats[1];
                Length = floats[2];
            }
        }

        public override string ToString()
        {
            string str = "";
            foreach (float f in floats)
            {
                str += f.ToString();
                str += " - ";
            }
            return str;
        }
    }
}
