using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    /// <summary>
    /// An object that corresponds to the standard testing response from the connected microcontroller.
    /// </summary>
    class TestingPacket : ReceivedData
    {
        public string Value { get; set; }

        public TestingPacket(byte[] data)
        {
            Value = "";
            if (data.Length != 20)
            {
                return;
            }
            else
            {
                for (int i = 0; i < data.Length; i++)
                {
                    Value += data[i];
                    Value += "|";
                }
                Value.Remove(Value.Length - 1);
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
