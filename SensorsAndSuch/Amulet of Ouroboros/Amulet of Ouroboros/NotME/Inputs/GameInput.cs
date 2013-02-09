using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;

namespace SensorsAndSuch.Inputs
{
    class GameInput
    {
        Dictionary<string, Input> inputs = new Dictionary<string, Input>();

        public MouseState CurrentMouseState;

        public Input MyInput(string theAction)
        {
            if (inputs.ContainsKey(theAction) == false)
            {
                inputs.Add(theAction, new Input());
            }

            return inputs[theAction];
        }

        public void BeginUpdate()
        {
            CurrentMouseState = Mouse.GetState();
            Input.BeginUpdate();
        }

        public void EndUpdate()
        {
            Input.EndUpdate();
        }

        public bool IsPressed(string theAction,
                              Rectangle theCurrentObjectLocation)
        {
            if (!inputs.ContainsKey(theAction))
            {
                return false;
            }

            return inputs[theAction].IsPressed(PlayerIndex.One,
                                               theCurrentObjectLocation);
        }

        public bool IsPressed(string theAction)
        {
            if (!inputs.ContainsKey(theAction))
            {
                return false;
            }
            return inputs[theAction].IsPressed(PlayerIndex.One);
        }

        public void AddGamePadInput(string theAction,
                                    Buttons theButton,
                                    bool isReleasedPreviously)
        {
            MyInput(theAction).AddGamepadInput(theButton,
                                               isReleasedPreviously);
        }

        public void AddKeyboardInput(string theAction,
                                     Keys theKey,
                                     bool isReleasedPreviously)
        {
            MyInput(theAction).AddKeyboardInput(theKey,
                                                isReleasedPreviously);
        }

        public bool CheckMousePress(Button button)
        {
            if (CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (CurrentMouseState.X <= button.Position.X + button.Width &&
                    CurrentMouseState.X >= button.Position.X &&
                    CurrentMouseState.Y <= button.Position.Y + button.Height &&
                    CurrentMouseState.Y >= button.Position.Y)
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool CheckMouseOver(Vector2 pos, int width, int height)
        {

            if (CurrentMouseState.X <= pos.X + width &&
                CurrentMouseState.X >= pos.X &&
                CurrentMouseState.Y <= pos.Y + height &&
                CurrentMouseState.Y >= pos.Y)
            {
                return true;
            }
            return false;
        }

        public bool CheckMousePress(Rectangle rect)
        {
            if (CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (CurrentMouseState.X <= rect.X + rect.Width &&
                    CurrentMouseState.X >= rect.X &&
                    CurrentMouseState.Y <= rect.Y + rect.Height &&
                    CurrentMouseState.Y >= rect.Y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckMouseRelease(Button button)
        {
            if (CurrentMouseState.LeftButton == ButtonState.Released)
            {
                if (CurrentMouseState.X <= button.Position.X + button.Width &&
                    CurrentMouseState.X >= button.Position.X &&
                    CurrentMouseState.Y <= button.Position.Y + button.Height &&
                    CurrentMouseState.Y >= button.Position.Y)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
