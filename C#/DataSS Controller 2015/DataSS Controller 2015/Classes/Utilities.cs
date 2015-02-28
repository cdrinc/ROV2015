using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    /// <summary>
    /// Contains extension methods and provides low-level computation utilities.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Maps a value within one range to a corresponding value in a different range.
        /// </summary>
        /// <param name="x">Variable to be mapped.</param>
        /// <param name="in_min">Minimum possible value of the variable, pre-map.</param>
        /// <param name="in_max">Maximum possible value of the variable, pre-map.</param>
        /// <param name="out_min">Minimum possible value of the variable, post-map.</param>
        /// <param name="out_max">Maximum possible value of the variable, post-map.</param>
        /// <returns>Returns the newly mapped value.</returns>
        public static float Map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (((x - in_min) * (out_max - out_min)) / (in_max - in_min)) + out_min;
        }

        /// <summary>
        /// Encapsulates the Map function to correctly map variable-speed, variable-direction motor values.
        /// </summary>
        /// <param name="val">Value to be mapped.</param>
        /// <returns>Returns the mapped value.</returns>
        public static float MapStick(float val)
        {
            float f;
            if (val < 0)
            {
                val = Math.Abs(val);
                f = Map(val, 0, 1, 1, 126);
            }
            else if (val > 0)
            {
                f = Map(val, 0, 1, 128, 255);
            }
            else
            {
                f = 0;
            }

            return f;
        }

        /// <summary>
        /// Searches the outer list for the exact sequence contained in the passed list.
        /// </summary>
        /// <typeparam name="T">Type contained in the list.</typeparam>
        /// <param name="outer">The list to be searched.</param>
        /// <param name="inner">The list containing the exact sequence to be searched for.</param>
        /// <returns>Returns a boolean value indicating whether or not the list contains the designated sequence.</returns>
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

        /// <summary>
        /// Adds a string to the listbox's list of items, then advances the SelectedIndex property to its maximum allowable value.
        /// </summary>
        /// <param name="listbox">The listbox to add the item to.</param>
        /// <param name="item">The string to add to the listbox.</param>
        public static void AddToEnd(this System.Windows.Forms.ListBox listbox, string item)
        {
            listbox.Items.Add(item);
            listbox.SelectedIndex = listbox.Items.Count - 1;
        }
    }
}
