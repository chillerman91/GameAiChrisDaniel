using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SensorsAndSuch.Sprites;

namespace SensorsAndSuch.Inputs
{
    class Input
    {
        Dictionary<Keys, bool> keyboardDefinedInputs = new Dictionary<Keys, bool>();
        static public KeyboardState CurrentKeyboardState;
        static public KeyboardState PreviousKeyboardState;

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public Input()
        {
        }

        static public void BeginUpdate()
        {
            CurrentKeyboardState = Keyboard.GetState(PlayerIndex.One);
        }

        static public void EndUpdate()
        {
            PreviousKeyboardState = CurrentKeyboardState;
        }

        public void AddKeyboardInput(Keys theKey, bool isReleasedPreviously)
        {
            if (keyboardDefinedInputs.ContainsKey(theKey))
            {
                keyboardDefinedInputs[theKey] = isReleasedPreviously;
                return;
            }
            keyboardDefinedInputs.Add(theKey, isReleasedPreviously);
        }

        public bool IsPressed(PlayerIndex thePlayerIndex)
        {
            return IsPressed(thePlayerIndex, null);
        }

        public bool IsPressed(PlayerIndex thePlayerIndex, Rectangle? theCurrentObjectLocation)
        {
            if (IsKeyboardInputPressed())
            {
                return true;
            }
            return false;
        }

        private bool IsKeyboardInputPressed()
        {
            foreach (Keys aKey in keyboardDefinedInputs.Keys)
            {
                if ((keyboardDefinedInputs[aKey] && CurrentKeyboardState.IsKeyDown(aKey) && !PreviousKeyboardState.IsKeyDown(aKey))
                    || (!keyboardDefinedInputs[aKey] && CurrentKeyboardState.IsKeyDown(aKey)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
