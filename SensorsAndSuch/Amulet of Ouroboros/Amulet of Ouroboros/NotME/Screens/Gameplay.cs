using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Amulet_of_Ouroboros.Texts;
using TiledLib;
using Amulet_of_Ouroboros.Maps;
using Amulet_of_Ouroboros.Mobs;
using FarseerPhysics.SamplesFramework;
//using Amulet_of_Ouroboros.Screens.Screen.ChangeScreen;
using Amulet_of_Ouroboros.FrameWork;

namespace Amulet_of_Ouroboros.Screens
{
    class Gameplay : SensorScreen
    {
        Background background;
        HUDPlayerInfo HUDPlayerInfo;
        Player player;
        Button leftButton;
        Button rightButton;
        Button upButton;
        Button downButton;

        Button attackButton;
        Button skipButton;
        Button potionButton;

        #region Pregame
        int charAmt;
        char[] enteredName;
        bool playerCreated;
        KeyboardState currentState;
        char keydown;
        Texture2D textenterbox;
        Text enterplayernameText;
        bool nameComplete;
        Text playername;
        Button confirmnameButton;
        Text chooseclasstext;
        Button paladinButton;
        Button casterButton;
        Button rogueButton;
        bool classComplete;
        Texture2D[] storytexts;
        int storyCount;
        Button storyContinueButton;
        #endregion

        int tick = 0;
        GraphicsDevice Device;
        public Gameplay(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics, GraphicsDevice Device)
            : base(game, batch, changeScreen, graphics)
        {
            this.Device = Device;
        }

        protected override void SetupInputs()
        {

        }

        public override void Activate()
        {

        }

        protected override void LoadScreenContent(ContentManager content)
        {
            base.LoadContent(content);
            World.Gravity = new Vector2(0f, 0f);
            background = new Background(content, "Images/GameplayBackground");

            Globals.SetGeneral(content, Device, World);
            Globals.AssetCreatorr.LoadContent(content);
            Globals.SetLevelSpecific(new MobManager(), new RandomMap());
            player = new Player(content);
            Globals.player = player;
            Globals.Mobs.AddPlayer(player);
            for (int i = 0; i < 0; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Snake, Globals.map.GetRandomFreePos());
            for (int i = 0; i < 0; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Snake, Globals.map.GetRandomFreePos(), Globals.rand.Next(100) + 20);
            for (int i = 0; i < 10; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Boar, Globals.map.GetRandomFreePos(), Globals.rand.Next(8) + 2);

            HUDPlayerInfo = new HUDPlayerInfo(content, player);
            leftButton = new Button(content, "", new Vector2(760, 705), Color.White, Color.White, "Images/LeftButton");
            rightButton = new Button(content, "", new Vector2(940, 705), Color.White, Color.White, "Images/RightButton");
            upButton = new Button(content, "", new Vector2(850, 650), Color.White, Color.White, "Images/UpButton");
            downButton = new Button(content, "", new Vector2(850, 705), Color.White, Color.White, "Images/DownButton");

            attackButton = new Button(content, "Warp", new Vector2(450, 705), Color.Blue, Color.White);
            skipButton = new Button(content, "Skip", new Vector2(450, 650), Color.Blue, Color.White);
            potionButton = new Button(content, "Potion", new Vector2(ScreenWidth / 2 + 220, 400), Color.Blue, Color.White);
        }
        private Boolean GameStart = false;
        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {

            base.Update(gameTime);
            //if (player.CurrentHP < 1)
            //{
            //    changeScreenDelegate(ScreenState.GameOver);
            //    this.LoadScreenContent(content);
            //}

            if (GameStart)
            {
              GameplayTurnCheck();
            }
            else
            {
                  player.CreatePlayer(0, "Shadow Regulus");
                  playerCreated = true;
                  GameStart = true;
           }
        }

        public char GetCharFromKeyboard(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.A))
                keydown = 'a';
            if (state.IsKeyDown(Keys.B) )
                keydown = 'b';
            if (state.IsKeyDown(Keys.C) )
                keydown = 'c';
            if (state.IsKeyDown(Keys.D) )
                keydown = 'd';
            if (state.IsKeyDown(Keys.E) )
                keydown = 'e';
            if (state.IsKeyDown(Keys.F) )
                keydown = 'f';
            if (state.IsKeyDown(Keys.G) )
                keydown = 'g';
            if (state.IsKeyDown(Keys.H) )
                keydown = 'h'; 
            if (state.IsKeyDown(Keys.I) )
                keydown = 'i';
            if (state.IsKeyDown(Keys.J) )
                keydown = 'j';
            if (state.IsKeyDown(Keys.K) )
                keydown = 'k';
            if (state.IsKeyDown(Keys.L) )
                keydown = 'l';
            if (state.IsKeyDown(Keys.M) )
                keydown = 'm';
            if (state.IsKeyDown(Keys.N) )
                keydown = 'n';
            if (state.IsKeyDown(Keys.O) )
                keydown = 'o';
            if (state.IsKeyDown(Keys.P) )
                keydown = 'p';
            if (state.IsKeyDown(Keys.Q) )
                keydown = 'q';
            if (state.IsKeyDown(Keys.R) )
                keydown = 'r';
            if (state.IsKeyDown(Keys.S) )
                keydown = 's';
            if (state.IsKeyDown(Keys.T) )
                keydown = 't';
            if (state.IsKeyDown(Keys.U) )
                keydown = 'u';
            if (state.IsKeyDown(Keys.V) )
                keydown = 'v';
            if (state.IsKeyDown(Keys.W) )
                keydown = 'w';
            if (state.IsKeyDown(Keys.X) )
                keydown = 'x';
            if (state.IsKeyDown(Keys.Y) )
                keydown = 'y';
            if (state.IsKeyDown(Keys.Z))
                keydown = 'z';

            if (state.IsKeyUp(Keys.A) && keydown == 'a')
            {
                keydown = (char)0;
                return 'a';
            }
            if (state.IsKeyUp(Keys.B) && keydown == 'b')
            {
                keydown = (char)0;
                return 'b';
            }
            if (state.IsKeyUp(Keys.C) && keydown == 'c')
            {
                keydown = (char)0;
                return 'c';
            }
            if (state.IsKeyUp(Keys.D) && keydown == 'd')
            {
                keydown = (char)0;
                return 'd';
            }
            if (state.IsKeyUp(Keys.E) && keydown == 'e')
            {
                keydown = (char)0;
                return 'e';
            }
            if (state.IsKeyUp(Keys.F) && keydown == 'f')
            {
                keydown = (char)0;
                return 'f';
            }
            if (state.IsKeyUp(Keys.G) && keydown == 'g')
            {
                keydown = (char)0;
                return 'g';
            }
            if (state.IsKeyUp(Keys.H) && keydown == 'h')
            {
                keydown = (char)0;
                return 'h';
            }
            if (state.IsKeyUp(Keys.I) && keydown == 'i')
            {
                keydown = (char)0;
                return 'i';
            }
            if (state.IsKeyUp(Keys.J) && keydown == 'j')
            {
                keydown = (char)0;
                return 'j';
            }
            if (state.IsKeyUp(Keys.K) && keydown == 'k')
            {
                keydown = (char)0;
                return 'k';
            }
            if (state.IsKeyUp(Keys.L) && keydown == 'l')
            {
                keydown = (char)0;
                return 'l';
            }
            if (state.IsKeyUp(Keys.M) && keydown == 'm')
            {
                keydown = (char)0;
                return 'm';
            }
            if (state.IsKeyUp(Keys.N) && keydown == 'n')
            {
                keydown = (char)0;
                return 'n';
            }
            if (state.IsKeyUp(Keys.O) && keydown == 'o')
            {
                keydown = (char)0;
                return 'o';
            }
            if (state.IsKeyUp(Keys.P) && keydown == 'p')
            {
                keydown = (char)0;
                return 'p';
            }
            if (state.IsKeyUp(Keys.Q) && keydown == 'q')
            {
                keydown = (char)0;
                return 'q';
            }
            if (state.IsKeyUp(Keys.R) && keydown == 'r')
            {
                keydown = (char)0;
                return 'r';
            }
            if (state.IsKeyUp(Keys.S) && keydown == 's')
            {
                keydown = (char)0;
                return 's';
            }
            if (state.IsKeyUp(Keys.T) && keydown == 't')
            {
                keydown = (char)0;
                return 't';
            }
            if (state.IsKeyUp(Keys.U) && keydown == 'u')
            {
                keydown = (char)0;
                return 'u';
            }
            if (state.IsKeyUp(Keys.V) && keydown == 'v')
            {
                keydown = (char)0;
                return 'v';
            }
            if (state.IsKeyUp(Keys.W) && keydown == 'w')
            {
                keydown = (char)0;
                return 'w';
            }
            if (state.IsKeyUp(Keys.X) && keydown == 'x')
            {
                keydown = (char)0;
                return 'x';
            }
            if (state.IsKeyUp(Keys.Y) && keydown == 'y')
            {
                keydown = (char)0;
                return 'y';
            }
            if (state.IsKeyUp(Keys.Z) && keydown == 'z')
            {
                keydown = (char)0;
                return 'z';
            }

            return (char)0;
        }
        
        private void GameplayTurnCheck()
        {
            ++tick;
            if (tick % 2 == 0){
               UpdateAll();
            }
            player.UpdatePosition();
            HUDPlayerInfo.Update(player);

            if (input.PreviousMouseState.LeftButton == ButtonState.Pressed && input.CheckMouseRelease(leftButton))
            {
                if (player.MakeMove(player.GridPos.X - 1, player.GridPos.Y ))
                    UpdateAll();
            }
            else if (input.PreviousMouseState.LeftButton == ButtonState.Pressed && input.CheckMouseRelease(rightButton) )
            {
                if (player.MakeMove(player.GridPos.X + 1, player.GridPos.Y))
                    UpdateAll();
            }
            else if (input.PreviousMouseState.LeftButton == ButtonState.Pressed && input.CheckMouseRelease(upButton))
            {
                if (player.MakeMove(player.GridPos.X, player.GridPos.Y - 1))
                    UpdateAll();
            }
            else if (input.PreviousMouseState.LeftButton == ButtonState.Pressed && input.CheckMouseRelease(downButton) )
            {
                if (player.MakeMove(player.GridPos.X, player.GridPos.Y + 1))
                    UpdateAll();
            }
            else if (input.PreviousMouseState.LeftButton == ButtonState.Pressed && input.CheckMouseRelease(attackButton))
            {
                player.Warp();
            }
            else if (input.PreviousMouseState.LeftButton == ButtonState.Pressed || input.CheckMouseRelease(skipButton))
            {
                //player.Rest();
                //UpdateAll();
            }
            Globals.Mobs.Update(input);
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
           // HUDPlayerInfo.Draw(batch);
            //leftButton.Draw(batch);
            //rightButton.Draw(batch);
            //upButton.Draw(batch);
            //downButton.Draw(batch);
        }
    }
}
