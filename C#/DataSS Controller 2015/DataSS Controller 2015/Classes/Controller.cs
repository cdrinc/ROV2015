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

        //public fields that are updated with control values as polling is done
        //all integers are either zero or one, false and true respectively
        //all floats are between 0 and 1
        public Vector2 LS = Vector2.Zero;
        public Vector2 RS = Vector2.Zero;
        public float LT = 0;
        public float RT = 0;
        public int A = 0;
        public int B = 0;
        public int X = 0;
        public int Y = 0;
        public int RB = 0;
        public int LB = 0;
        public int DUp = 0;
        public int DLeft = 0;
        public int DRight = 0;
        public int DDown = 0;
        public int LSClick = 0;
        public int RSClick = 0;
        public int Start = 0;
        public int Back = 0;
        public int BigButton = 0;

        /// <summary>
        /// Non-implemented polling function (overrided in child classes).
        /// </summary>
        public virtual void BeginPolling()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Non-implemented polling function (overrided in child classes).
        /// </summary>
        public virtual void EndPolling()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Non-implemented polling function (overrided in child classes).
        /// </summary>
        public virtual bool IsPolling()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnInputChanged()
        {
            //if (InputChanged != null)
                InputChanged(this, new ControllerEventArgs());
        }
    }
}
