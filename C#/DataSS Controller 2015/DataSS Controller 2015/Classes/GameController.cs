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
        public event EventHandler InputChanged;

        private GamePadState padState;

        Thread poll;

        public GameController()
        {
            poll = new Thread(new ThreadStart(polling));
            poll.Start();
        }

        void polling()
        {
            for (; ; )
            {
                padState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
                bool flag = false;
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
                if (A != (padState.Buttons.A == 0 ? false : true))
                {
                    A = (padState.Buttons.A == 0 ? false : true);
                    flag = true;
                }
                if (B != (padState.Buttons.B == 0 ? false : true))
                {
                    B = (padState.Buttons.B == 0 ? false : true);
                    flag = true;
                }
                if (X != (padState.Buttons.X == 0 ? false : true))
                {
                    X = (padState.Buttons.X == 0 ? false : true);
                    flag = true;
                }
                if (Y != (padState.Buttons.Y == 0 ? false : true))
                {
                    Y = (padState.Buttons.Y == 0 ? false : true);
                    flag = true;
                }
                if (LB != (padState.Buttons.LeftShoulder == 0 ? false : true))
                {
                    LB = (padState.Buttons.LeftShoulder == 0 ? false : true);
                    flag = true;
                }
                if (RB != (padState.Buttons.RightShoulder == 0 ? false : true))
                {
                    RB = (padState.Buttons.RightShoulder == 0 ? false : true);
                    flag = true;
                }
                if (flag)
                {
                    InputChanged(this, new EventArgs());
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
