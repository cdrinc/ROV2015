using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1516:ElementsMustBeSeparatedByBlankLine", Justification = "Reviewed. Suppression is OK here.")]

    /// <summary>
    /// Stores and serializes the packet data for transmission through the Connection class.
    /// </summary>
    public class SentData
    {
        public byte Meta 
        {
            get { return 0x00; }
            set {  } 
        }
        public byte LSY { get; set; }
        public byte LSX { get; set; }
        public byte RSY { get; set; }
        public byte RSX { get; set; }
        public byte LT { get; set; }
        public byte RT { get; set; }
        public byte A { get; set; }
        public byte B { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte RB { get; set; }
        public byte LB { get; set; }
        public byte DUp { get; set; }
        public byte DDown { get; set; }
        public byte DLeft { get; set; }
        public byte DRight { get; set; }
        public byte LSClick { get; set; }
        public byte RSClick { get; set; }
        public byte Start { get; set; }
        public byte Back { get; set; }

        /// <summary>
        /// Converts the data stored in the class to a byte array in the correct format for transmission.
        /// </summary>
        /// <returns>A byte array containing the data to be sent.</returns>
        public byte[] Serialize()
        {
            List<byte> byteList = new List<byte>() { Meta, LSY, LSX, RSY, RSX, LT, RT, A, B, X, Y, RB, LB, DUp, DDown, DRight, DLeft, LSClick, RSClick, Start, Back };
            byte[] byteArr = byteList.ToArray();
            return byteArr;
        }
    }
}
