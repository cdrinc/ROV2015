using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSS_Controller_2015.Classes
{
    public class Controller
    {
        public delegate void ControllerHandler(object sender, ControllerEventArgs e);
        public event ControllerHandler InputChanged;

        public Vector2 LS = Vector2.Zero;
        public Vector2 RS = Vector2.Zero;
        public float LT = 0;
        public float RT = 0;
        public bool A = false;
        public bool B = false;
        public bool X = false;
        public bool Y = false;
        public bool RB = false;
        public bool LB = false;

        public virtual void BeginPolling()
        {

        }

        protected virtual void OnInputChanged()
        {
            //if (InputChanged != null)
                InputChanged(this, new ControllerEventArgs());
        }
    }
}
