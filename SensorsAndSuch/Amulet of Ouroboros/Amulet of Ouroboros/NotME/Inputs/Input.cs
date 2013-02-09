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
        Dictionary<Buttons, bool> gamepadDefinedInputs = new Dictionary<Buttons, bool>();
        
        static public Dictionary<PlayerIndex, GamePadState> CurrentGamePadState = new Dictionary<PlayerIndex, GamePadState>();
        static public Dictionary<PlayerIndex, GamePadState> PreviousGamePadState = new Dictionary<PlayerIndex, GamePadState>();
        static public KeyboardState CurrentKeyboardState;
        static public KeyboardState PreviousKeyboardState;
        static public Dictionary<PlayerIndex, bool> GamepadConnectionState = new Dictionary<PlayerIndex, bool>();


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

        public void AddGamepadInput(Buttons theButton, bool isReleasedPreviously)
        {
            if (gamepadDefinedInputs.ContainsKey(theButton))
            {
                gamepadDefinedInputs[theButton] = isReleasedPreviously;
                return;
            }
            gamepadDefinedInputs.Add(theButton, isReleasedPreviously);
        }

        static public bool IsConnected(PlayerIndex thePlayerIndex)
        {
            return CurrentGamePadState[thePlayerIndex].IsConnected;
        }

        public bool IsPressed(PlayerIndex thePlayerIndex)
        {
            return IsPressed(thePlayerIndex, null);
        }

        public bool IsPressed(PlayerIndex thePlayerIndex, Rectangle? theCurrentObjectLocation)
        {
            if (IsKeyboardInputPressed() || IsGamepadInputPressed(thePlayerIndex))
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

        private bool IsGamepadInputPressed(PlayerIndex thePlayerIndex)
        {
            foreach (Buttons aButton in gamepadDefinedInputs.Keys)
            {
                if ((gamepadDefinedInputs[aButton] && CurrentGamePadState[thePlayerIndex].IsButtonDown(aButton) && !PreviousGamePadState[thePlayerIndex].IsButtonDown(aButton))
                    || (!gamepadDefinedInputs[aButton] && CurrentGamePadState[thePlayerIndex].IsButtonDown(aButton)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
