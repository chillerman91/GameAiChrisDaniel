using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Amulet_of_Ouroboros.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Amulet_of_Ouroboros.FrameWork
{
    class SensorScreen: Screen
    {
        protected World World;

        public SensorScreen(Game game, SpriteBatch batch, Amulet_of_Ouroboros.Screens.Screen.ChangeScreen changeScreen, GraphicsDeviceManager graphics)
            : base(game, batch, changeScreen, graphics)
        {
        }

        public void LoadContent()
        {
            // base.LoadContent();

            //We enable diagnostics to show get values for our performance counters.
            //Settings.EnableDiagnostics = true;

            if (World == null)
            {
                World = new World(Vector2.Zero);
            }
            else
            {
                World.Clear();
            }
            /*
            if (DebugView == null)
            {
                DebugView = new DebugViewXNA(World);
                DebugView.RemoveFlags(DebugViewFlags.Shape);
                DebugView.RemoveFlags(DebugViewFlags.Joint);
                DebugView.DefaultShapeColor = Color.White;
                DebugView.SleepingShapeColor = Color.LightGray;
                DebugView.LoadContent(ScreenManager.GraphicsDevice, ScreenManager.Content);
            }
            if (Camera == null)
            {
                Camera = new Camera2D(ScreenManager.GraphicsDevice);
            }
            else
            {
                Camera.ResetCamera();
            }

            */
            // Loading may take a while... so prevent the game from "catching up" once we finished loading
            //ScreenManager.Game.ResetElapsedTime();
        }
    }
}
