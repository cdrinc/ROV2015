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
    class SensorData : PacketResponse
    {
        public float GyroX;
        public float GyroY;
        public float GyroZ;
        public float AccelX;
        public float AccelY;
        public float AccelZ;
        public float CompassX;
        public float CompassY;
        public float CompassZ;
        public float Voltage;
        public float Length;
        public float Depth;
        
        private List<float> FloatList;

        public SensorData(byte[] data)
        {
            if (data.Length != 12)
            {
                return;
            }
            else
            {
                FloatList = new List<float>();
                for (int i = 0; i < data.Length; i++)
                {
                    //processing of packet data would go here
                    FloatList[i] = data[i];
                }

                GyroX = FloatList[0];
                GyroY = FloatList[1];
                GyroZ = FloatList[2];
                AccelX = FloatList[3];
                AccelY = FloatList[4];
                AccelZ = FloatList[5];
                CompassX = FloatList[6];
                CompassY = FloatList[7];
                CompassZ = FloatList[8];
                Voltage = FloatList[9];
                Length = FloatList[10];
                Depth = FloatList[11];
            }
        }

        public override string ToString()
        {
            string str = "";
            foreach (float f in FloatList)
            {
                str += f.ToString();
                str += " - ";
            }
            return str;
        }
    }
}
