using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;

namespace DataSS_Controller_2015.Classes
{
    public class ReceivedData : ISerializable
    {
        public float GyroX;
        public float GyroY;
        public float GyroZ;
        public float AccelX;
        public float AccelY;
        public float AccelZ;
        public float MagnetX;
        public float MagnetY;
        public float MagnetZ;
        public float Voltage;
        public float Length;
        public float Depth;
    }
}
