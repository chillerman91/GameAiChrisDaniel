using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using SensorsAndSuch.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.SamplesFramework;
using FarseerPhysics.Collision;
using FarseerPhysics.Common.Decomposition;
using Microsoft.Xna.Framework.Content;

namespace SensorsAndSuch.FrameWork
{
    class SensorScreen: Screen
    {
        protected World World;
        private GraphicsDevice _device;
        private Texture2D _materials;
        private BasicEffect _effect;

        public SensorScreen(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics)
            : base(game, batch, changeScreen, graphics)
        {
        }

        public void LoadContent(ContentManager contentManager)
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
        }
        public void Update(GameTime gameTime, bool otherScreenHasFocus = false, bool coveredByOtherScreen = false)
        {
            if (!coveredByOtherScreen && !otherScreenHasFocus)
            {
                // variable time step but never less then 30 Hz
                World.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            }
            else
            {
                World.Step(0f);
            }
            //Camera.Update(gameTime);
            //base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}
