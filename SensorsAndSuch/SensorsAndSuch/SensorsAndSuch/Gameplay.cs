using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using SensorsAndSuch.Texts;
using SensorsAndSuch.Maps;
using SensorsAndSuch.Mobs;
using FarseerPhysics.SamplesFramework;
//using SensorsAndSuch.Screens.Screen.ChangeScreen;
using SensorsAndSuch.FrameWork;

namespace SensorsAndSuch.Screens
{
    class Gameplay : SensorScreen
    {
        Background background;
        HUDPlayerInfo HUDPlayerInfo;
        Player player;

        int tick = 0;
        GraphicsDevice Device;
        public Gameplay(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics, GraphicsDevice Device)
            : base(game, batch, changeScreen, graphics)
        {
            this.Device = Device;
        }

        protected override void LoadScreenContent(ContentManager content)
        {
            base.LoadContent(content);
            World.Gravity = new Vector2(0f, 0f);
            background = new Background(content, "Images/GameplayBackground");
            Globals.SetGeneral(content, Device, World);
            Globals.AssetCreatorr.LoadContent(content);
            Globals.SetLevelSpecific(new MobManager(), new RandomMap());
            player = new Player(content, Globals.map.GetRandomFreePos());
            Globals.player = player;
            HUDPlayerInfo = new HUDPlayerInfo(content, player);
            for (int i = 0; i < 10; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, Globals.map.GetRandomFreePos(), Globals.rand.Next(8) + 2);
        }
        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {

            base.Update(gameTime);
            GameplayTurnCheck();
        }
        private void GameplayTurnCheck()
        {            
            if (!Inputs.Input.PreviousKeyboardState.IsKeyDown(Keys.P) &&Inputs.Input.CurrentKeyboardState.IsKeyDown(Keys.P))
            {
                paused = !paused;
            }
            if (paused) return;

            ++tick;
            if (tick % 2 == 0)
            {
               UpdateAll();
            }

            //HUDPlayerInfo.Update(player);
            if (input.PreviousMouseState.LeftButton == ButtonState.Pressed && input.CurrentMouseState.LeftButton == ButtonState.Released)
            {
                Globals.map.CreateBlock(input.CurrentMouseState.X, input.CurrentMouseState.Y);
            }

            if (input.PreviousMouseState.RightButton == ButtonState.Pressed && input.CurrentMouseState.RightButton == ButtonState.Released)
            {
                Vector2 pos = Globals.map.TranslateToPosRev(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                pos = new Vector2((int)pos.X, (int)pos.Y);
                if(Globals.map.isFree(pos))
                    Globals.Mobs.AddMonster(BaseMonster.MonTypes.Static, pos);
            }

            if (Inputs.Input.CurrentKeyboardState.IsKeyDown(Keys.A))
            {
                player.TakeTurn(Player.MoveOpt.RIGHT);
            }
            if (Inputs.Input.CurrentKeyboardState.IsKeyDown(Keys.D))
            {
                player.TakeTurn(Player.MoveOpt.LEFT);
            }
            if (Inputs.Input.CurrentKeyboardState.IsKeyDown(Keys.W))
            {
                player.TakeTurn(Player.MoveOpt.FORWARD);
            }
            if (Inputs.Input.CurrentKeyboardState.IsKeyDown(Keys.S))
            {
                player.TakeTurn(Player.MoveOpt.BACK);
            } 
            if (!Inputs.Input.PreviousKeyboardState.IsKeyDown(Keys.R) && Inputs.Input.CurrentKeyboardState.IsKeyDown(Keys.R))
            {
                player.Warp();
            }
            player.TakeTurn(Player.MoveOpt.NONE);

            //Globals.Mobs.Update(input);
        }

        private void UpdateAll() {
            Globals.map.Update();
            Globals.Mobs.UpdateMobs();
        }

        protected override void DrawScreen(SpriteBatch batch, DisplayOrientation screenOrientation)
        {
            Globals.map.Draw(batch);
            Globals.Mobs.Draw(batch);
            player.Draw(batch);
            //attackButton.Draw(batch);

            //skipButton.Draw(batch);
            //leftButton.Draw(batch);
            //rightButton.Draw(batch);
            //upButton.Draw(batch);
            //downButton.Draw(batch);
            HUDPlayerInfo.Draw(batch);
        }
    }
}
