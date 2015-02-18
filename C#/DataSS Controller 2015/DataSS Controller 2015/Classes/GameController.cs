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
            //ControllerData gamepadData = new ControllerData();

            //gamepadData.LS = Vector2.Zero;
            //gamepadData.RS = Vector2.Zero;
            //gamepadData.A = 0;
            //gamepadData.B = 0;
            //gamepadData.X = 0;
            //gamepadData.Y = 0;
            //gamepadData.RB = 0;
            //gamepadData.LB = 0;
            //gamepadData.DUp = 0;
            //gamepadData.DLeft = 0;
            //gamepadData.DRight = 0;
            //gamepadData.DDown = 0;
            //gamepadData.LSClick = 0;
            //gamepadData.RSClick = 0;
            //gamepadData.Start = 0;
            //gamepadData.Back = 0;
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

        /// <summary>
        /// Polls the gamepad to detect state changes.
        /// </summary>
        private void Polling()
        {
            ControllerData gamepadData = new ControllerData();
            while (true)
            {
                padState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
                bool flag = false;
                OnIncomingData();
                #region Sticks
                if (gamepadData.LS != padState.ThumbSticks.Left)
                {
                    gamepadData.LS = padState.ThumbSticks.Left;
                    flag = true;
                }

                if (gamepadData.RS != padState.ThumbSticks.Right)
                {
                    gamepadData.RS = padState.ThumbSticks.Right;
                    flag = true;
                }

                #endregion
                #region Triggers
                if (gamepadData.LT != padState.Triggers.Left)
                {
                    gamepadData.LT = padState.Triggers.Left;
                    flag = true;
                }

                if (gamepadData.RT != padState.Triggers.Right)
                {
                    gamepadData.RT = padState.Triggers.Right;
                    flag = true;
                }

                #endregion
                #region Buttons, Shoulders, DPad, Stick Clicks, Start/Select
                if (gamepadData.A != (int)padState.Buttons.A)
                {
                    gamepadData.A = (int)padState.Buttons.A;
                    flag = true;
                }

                if (gamepadData.B != (int)padState.Buttons.B)
                {
                    gamepadData.B = (int)padState.Buttons.B;
                    flag = true;
                }

                if (gamepadData.X != (int)padState.Buttons.X)
                {
                    gamepadData.X = (int)padState.Buttons.X;
                    flag = true;
                }

                if (gamepadData.Y != (int)padState.Buttons.Y)
                {
                    gamepadData.Y = (int)padState.Buttons.Y;
                    flag = true;
                }

                if (gamepadData.LB != (int)padState.Buttons.LeftShoulder)
                {
                    gamepadData.LB = (int)padState.Buttons.LeftShoulder;
                    flag = true;
                }

                if (gamepadData.RB != (int)padState.Buttons.RightShoulder)
                {
                    gamepadData.RB = (int)padState.Buttons.RightShoulder;
                    flag = true;
                }

                if (gamepadData.LSClick != (int)padState.Buttons.LeftStick)
                {
                    gamepadData.LSClick = (int)padState.Buttons.LeftStick;
                    flag = true;
                }

                if (gamepadData.RSClick != (int)padState.Buttons.RightStick)
                {
                    gamepadData.RSClick = (int)padState.Buttons.RightStick;
                    flag = true;
                }

                if (gamepadData.DUp != (int)padState.DPad.Up)
                {
                    gamepadData.DUp = (int)padState.DPad.Up;
                    flag = true;
                }

                if (gamepadData.DLeft != (int)padState.DPad.Left)
                {
                    gamepadData.DLeft = (int)padState.DPad.Left;
                    flag = true;
                }

                if (gamepadData.DRight != (int)padState.DPad.Right)
                {
                    gamepadData.DRight = (int)padState.DPad.Right;
                    flag = true;
                }

                if (gamepadData.DDown != (int)padState.DPad.Down)
                {
                    gamepadData.DDown = (int)padState.DPad.Down;
                    flag = true;
                }

                if (gamepadData.Start != (int)padState.Buttons.Start)
                {
                    gamepadData.Start = (int)padState.Buttons.Start;
                    flag = true;
                }

                if (gamepadData.Back != (int)padState.Buttons.Back)
                {
                    gamepadData.Back = (int)padState.Buttons.Back;
                    flag = true;
                }

                #endregion
                if (flag)
                    OnInputChanged(gamepadData);
            }
        }
    }
}
