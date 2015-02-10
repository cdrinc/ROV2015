using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Web.Script.Serialization;

namespace DataSS_Controller_2015.Classes
{
    public class GameController : Controller
    {

        private GamePadState padState;
        Thread poll;

        public GameController(TcpConnection con)
        {
            connection = con;
        }

        public override void BeginPolling()
        {
            poll = new Thread(new ThreadStart(polling));
            poll.Start();
        }

        public override void EndPolling()
        {
            if (poll != null)
                if (poll.IsAlive)
                    poll.Abort();
        }

        public override bool IsPolling()
        {
            if (poll != null)
                return poll.IsAlive;
            else
                return false;
        }

        void polling()
        {
            JavaScriptSerializer jSer = new JavaScriptSerializer();
            string incomingData = null;
            string jsonData;
            for (; ; )
            {
                padState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
                bool flag = false;
                if (connection != null)
                    incomingData = connection.ReadAllAvailable();
                if (incomingData != null && incomingData != "")
                {
                    flag = true;
                    try
                    {
                        jsonData = jSer.Deserialize<string>(incomingData);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
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
                #region Buttons, Shoulders, DPad, Stick Clicks, Start/Select
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
                if (LSClick != (int)padState.Buttons.LeftStick)
                {
                    LSClick = (int)padState.Buttons.LeftStick;
                    flag = true;
                }
                if (RSClick != (int)padState.Buttons.RightStick)
                {
                    RSClick = (int)padState.Buttons.RightStick;
                    flag = true;
                }
                if (DUp != (int)padState.DPad.Up)
                {
                    DUp = (int)padState.DPad.Up;
                    flag = true;
                }
                if (DLeft != (int)padState.DPad.Left)
                {
                    DLeft = (int)padState.DPad.Left;
                    flag = true;
                }
                if (DRight != (int)padState.DPad.Right)
                {
                    DRight = (int)padState.DPad.Right;
                    flag = true;
                }
                if (DDown != (int)padState.DPad.Down)
                {
                    DDown = (int)padState.DPad.Down;
                    flag = true;
                } 
                if (Start != (int)padState.Buttons.Start)
                {
                    Start = (int)padState.Buttons.Start;
                    flag = true;
                }
                if (Back != (int)padState.Buttons.Back)
                {
                    Back = (int)padState.Buttons.Back;
                    flag = true;
                }
                #endregion
                if (flag)
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
