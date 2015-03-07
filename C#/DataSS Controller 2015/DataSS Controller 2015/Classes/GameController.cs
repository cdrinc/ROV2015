using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DataSS_Controller_2015.Classes
{
    /// <summary>
    /// Provides functionality for a connected gamepad.
    /// </summary>
    public class GameController : Controller
    {
        private GamePadState padState;
        private Thread poll;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// </summary>
        public GameController() 
        {
            this.ls = Vector2.Zero;
            this.rs = Vector2.Zero;
            this.A = 0;
            this.B = 0;
            this.X = 0;
            this.Y = 0;
            this.RB = 0;
            this.LB = 0;
            this.DUp = 0;
            this.DLeft = 0;
            this.DRight = 0;
            this.DDown = 0;
            this.LSClick = 0;
            this.RSClick = 0;
            this.Start = 0;
            this.Back = 0;
        }

        /// <summary>
        /// Begins polling of the connected gamepad on another thread.
        /// </summary>
        public override void BeginPolling()
        {
            poll = new Thread(new ThreadStart(Polling));
            poll.Start();
        }

        /// <summary>
        /// Stops polling of the connected gamepad.
        /// </summary>
        public override void EndPolling()
        {
            if (poll != null)
                if (poll.IsAlive)
                    poll.Abort();
        }

        /// <summary>
        /// Determines whether or not the Polling method is running.
        /// </summary>
        /// <returns>Returns a boolean value indicating whether or not the Polling method is running.</returns>
        public override bool IsPolling()
        {
            if (poll != null)
                return poll.IsAlive;
            else
                return false;
        }

        public override void Poll(out bool changed)
        {
            padState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            bool flag = false;
            #region Sticks
            if (ls != padState.ThumbSticks.Left)
            {
                ls = padState.ThumbSticks.Left;
                flag = true;
            }

            if (rs != padState.ThumbSticks.Right)
            {
                rs = padState.ThumbSticks.Right;
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
            changed = flag;
        }

        /// <summary>
        /// Polls the gamepad to detect state changes.
        /// </summary>
        private void Polling()
        {
            while (true)
            {
                padState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
                bool flag = false;
                OnIncomingData();
                #region Sticks
                if (ls != padState.ThumbSticks.Left)
                {
                    ls = padState.ThumbSticks.Left;
                    flag = true;
                }

                if (rs != padState.ThumbSticks.Right)
                {
                    rs = padState.ThumbSticks.Right;
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
}
