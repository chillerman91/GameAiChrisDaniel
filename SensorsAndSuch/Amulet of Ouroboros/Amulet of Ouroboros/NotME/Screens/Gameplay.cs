using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Amulet_of_Ouroboros.Texts;
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

        //public InputHelper input;
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
            player = new Player(content, Globals.map.GetRandomFreePos());
            Globals.player = player;
            //Globals.Mobs.AddPlayer(player);
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

        private void GameplayTurnCheck()
        {
            ++tick;
            if (tick % 2 == 0)
            {
               UpdateAll();
            }

            //HUDPlayerInfo.Update(player);

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
           // HUDPlayerInfo.Draw(batch);
            //leftButton.Draw(batch);
            //rightButton.Draw(batch);
            //upButton.Draw(batch);
            //downButton.Draw(batch);
        }

        public void PlaceBadGuy(EventArgs e)
        {

        }
    }
}
