using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace DataSS_Controller_2015.Classes
{
    public class KeyboardController : Controller
    {
        //public delegate void ControllerHandler(object sender, ControllerEventArgs e);
        //public event ControllerHandler InputChanged;

        private KeyboardState keyboardState;

        Thread poll;

        public KeyboardController()
        {
            poll = new Thread(new ThreadStart(polling));
            poll.Start();
        }

        void polling()
        {
            while (true)
            {
                keyboardState = Keyboard.GetState();
                bool flag = false;
                #region left stick equivalents
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    LS.X = -1;
                    flag = true;
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    LS.X = 1;
                    flag = true;
                }
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    LS.Y = 1;
                    flag = true;
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    LS.Y = -1;
                    flag = true;
                }
                if (keyboardState.IsKeyUp(Keys.A) && keyboardState.IsKeyUp(Keys.D) && LS.X != 0)
                {
                    LS.X = 0;
                    flag = true;
                }
                if (keyboardState.IsKeyUp(Keys.W) && keyboardState.IsKeyUp(Keys.S) && LS.Y != 0)
                {
                    LS.Y = 0;
                    flag = true;
                }
                #endregion
                #region right stick equivalents
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    RS.X = -1;
                    flag = true;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    RS.X = 1;
                    flag = true;
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    RS.Y = 1;
                    flag = true;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    RS.Y = -1;
                    flag = true;
                }
                if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right) && RS.X != 0)
                {
                    RS.X = 0;
                    flag = true;
                }
                if (keyboardState.IsKeyUp(Keys.Down) && keyboardState.IsKeyUp(Keys.Up) && RS.Y != 0)
                {
                    RS.Y = 0;
                    flag = true;
                }
                #endregion
                #region trigger equivalents
                if (keyboardState.IsKeyDown(Keys.Q))
                {
                    LT = 1;
                    flag = true;
                }
                if (keyboardState.IsKeyDown(Keys.E))
                {
                    RT = 1;
                    flag = true;
                }
                if (keyboardState.IsKeyUp(Keys.Q) && LT != 0)
                {
                    LT = 0;
                    flag = true;
                }
                if (keyboardState.IsKeyUp(Keys.E) && LT != 0)
                {
                    RT = 0;
                    flag = true;
                }
#endregion
                #region button equivalents
                if (A != keyboardState.IsKeyDown(Keys.M))
                {
                    A = keyboardState.IsKeyDown(Keys.M);
                    flag = true;
                }
                if (B != keyboardState.IsKeyDown(Keys.OemPeriod))
                {
                    B = keyboardState.IsKeyDown(Keys.OemPeriod);
                    flag = true;
                }
                if (X != keyboardState.IsKeyDown(Keys.OemComma))
                {
                    X = keyboardState.IsKeyDown(Keys.OemComma);
                    flag = true;
                }
                if (Y != keyboardState.IsKeyDown(Keys.Divide))
                {
                    Y = keyboardState.IsKeyDown(Keys.Divide);
                    flag = true;
                }
                if (LB != keyboardState.IsKeyDown(Keys.Z))
                {
                    LB = keyboardState.IsKeyDown(Keys.Z);
                    flag = true;
                }
                if (RB != keyboardState.IsKeyDown(Keys.Z))
                {
                    RB = keyboardState.IsKeyDown(Keys.Z);
                    flag = true;
                }
                #endregion
                if (flag)
                {
                    OnInputChanged();
                }
            }
        }

        public class ControllerEventArgs : EventArgs
        {
            public ControllerEventArgs() :
                base() { }
        }
    }
}
