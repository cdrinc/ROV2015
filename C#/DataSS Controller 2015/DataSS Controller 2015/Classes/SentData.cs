using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;

namespace DataSS_Controller_2015.Classes
{
    /// <summary>
    /// 0   -   ls y
    /// 1   -   ls x
    /// 2   -   rs y
    /// 3   -   rs x
    /// 4   -   lt
    /// 5   -   rt
    /// 6   -   a
    /// 7   -   b
    /// 8   -   x
    /// 9   -   y
    /// 10  -   rb
    /// 11  -   lb
    /// 12  -   dup
    /// 13  -   ddown
    /// 14  -   dleft
    /// 15  -   dright
    /// 16  -   lsclick
    /// 17  -   rsclick
    /// 18  -   start
    /// 19  -   back
    /// </summary>
    public class SentData : ISerializable
    {
        public byte LSY;
        public byte LSX;
        public byte RSY;
        public byte RSX;
        public byte LT;
        public byte RT;
        public byte A;
        public byte B;
        public byte X;
        public byte Y;
        public byte RB;
        public byte LB;
        public byte DUp;
        public byte DDown;
        public byte DLeft;
        public byte DRight;
        public byte LSClick;
        public byte RSClick;
        public byte Start;
        public byte Back;
    }
}
