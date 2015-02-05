using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace DataSS_Controller_2015.Classes
{
    public class GameController : Controller
    {

        private GamePadState padState;

        Thread poll;

        public GameController()
        {
            
        }

        public override void BeginPolling()
        {
            //threading doesnt seem to work with keyboards
            poll = new Thread(new ThreadStart(polling));
            poll.Start();
            //polling();
        }

        void polling()
        {
            for (; ; )
            {
                padState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
                bool flag = false;
                #region Sticks
                if (LS != padState.ThumbSticks.Left)
                {
                    LS = padState.ThumbSticks.Left;
                    flag = true;
                }
                if (RS != padState.ThumbSticks.Right)
                {
                    RS = padState.ThumbSticks.Right;
                    flag = true;
                }
                #endregion
                #region Triggers
                if (LT != padState.Triggers.Left)
                {
                    LT = padState.Triggers.Left;
                    flag = true;
                }
                if (RT != padState.Triggers.Right)
                {
                    RT = padState.Triggers.Right;
                    flag = true;
                }
                #endregion
                if (A != (int)padState.Buttons.A)
                {
                    A = (int)padState.Buttons.A;
                    flag = true;
                }
                if (B != (int)padState.Buttons.B)
                {
                    B = (int)padState.Buttons.B;
                    flag = true;
                }
                if (X != (int)padState.Buttons.X)
                {
                    X = (int)padState.Buttons.X;
                    flag = true;
                }
                if (Y != (int)padState.Buttons.Y)
                {
                    Y = (int)padState.Buttons.Y;
                    flag = true;
                }
                if (LB != (int)padState.Buttons.LeftShoulder)
                {
                    LB = (int)padState.Buttons.LeftShoulder;
                    flag = true;
                }
                if (RB != (int)padState.Buttons.RightShoulder)
                {
                    RB = (int)padState.Buttons.RightShoulder;
                    flag = true;
                }
                if (flag)
                {
                    OnInputChanged();
                }
                 
            }
        }
    }

    public class ControllerEventArgs : EventArgs
    {
        public ControllerEventArgs() :
            base() { }
    }
}
