using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DataSS_Controller_2015.Classes
{
    public class ControllerData
    {
        private Vector2 ls;
        private Vector2 rs;

        public Vector2 LS
        {
            get { return ls; }
            set { ls = value; }
        }

        public Vector2 RS
        {
            get { return rs; }
            set { rs = value; }
        }

        public float LT { get; set; }
        public float RT { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int RB { get; set; }
        public int LB { get; set; }
        public int DUp { get; set; }
        public int DLeft { get; set; }
        public int DRight { get; set; }
        public int DDown { get; set; }
        public int LSClick { get; set; }
        public int RSClick { get; set; }
        public int Start { get; set; }
        public int Back { get; set; }
    }
}
