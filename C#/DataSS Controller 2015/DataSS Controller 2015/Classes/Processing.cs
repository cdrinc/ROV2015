using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    public static class Processing
    {
        /// <summary>
        /// Borrowed from arduino, maps a value within one range to a corresponding value in a different range.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="in_min"></param>
        /// <param name="in_max"></param>
        /// <param name="out_min"></param>
        /// <param name="out_max"></param>
        /// <returns>Returns the newly mapped value.</returns>
        public static float Map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static bool ContainsSequence<T>(this List<T> outer, List<T> inner)
        {
            var innerCount = inner.Count;
            if (innerCount > outer.Count)
                return false;

            for (int i = 0; i <= outer.Count - innerCount; i++)
            {
                bool isMatch = true;
                for (int x = 0; x < innerCount; x++)
                {
                    if (!outer[i + x].Equals(inner[x]))
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch) return true;
            }

            return false;
        }
    }


}
