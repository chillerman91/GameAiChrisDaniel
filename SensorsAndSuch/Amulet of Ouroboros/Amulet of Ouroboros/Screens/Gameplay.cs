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

namespace Amulet_of_Ouroboros.Screens
{
    class Gameplay : PhysicsGameScreen
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
        Minion minion;
        Boss boss;
        bool inBattle;
        bool bossBattle;
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
        TileLayer tileLayer;
        TileLayer copyTileLayer;
        Tile blankTile;
        Tile walkableTile;
        Tile enemykilledTile;
        Tile enemyTile;
        Tile exitTile;
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

            World.Gravity = new Vector2(0f, 20f);
            background = new Background(content, "Images/GameplayBackground");

            Globals.SetGeneral( content, Device);
            Globals.SetLevelSpecific(new MobManager(), new RandomMap());
            player = new Player(content);
            Globals.player = player;
            Globals.Mobs.AddPlayer(player);
            for (int i = 0; i < 120; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Snake, Globals.map.GetRandomFreePos());
            for (int i = 0; i < 8; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Snake, Globals.map.GetRandomFreePos(), Globals.rand.Next(100) + 20);
            for (int i = 0; i < 150; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Boar, Globals.map.GetRandomFreePos(), Globals.rand.Next(8) + 2);

            HUDPlayerInfo = new HUDPlayerInfo(content, player);
            leftButton = new Button(content, "", new Vector2(760, 705), Color.White, Color.White, "Images/LeftButton");
            rightButton = new Button(content, "", new Vector2(940, 705), Color.White, Color.White, "Images/RightButton");
            upButton = new Button(content, "", new Vector2(850, 650), Color.White, Color.White, "Images/UpButton");
            downButton = new Button(content, "", new Vector2(850, 705), Color.White, Color.White, "Images/DownButton");

            attackButton = new Button(content, "Warp", new Vector2(450, 705), Color.Blue, Color.White);
            skipButton = new Button(content, "Skip", new Vector2(450, 650), Color.Blue, Color.White);
            potionButton = new Button(content, "Potion", new Vector2(ScreenWidth / 2 + 220, 400), Color.Blue, Color.White);
            minion = new Minion(content);
            boss = new Boss(content);
            inBattle = false;
            bossBattle = false;
            
            #region Pregame
            playerCreated = false;
            keydown = (char)0;
            enterplayernameText = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "Enter Characters Name:", new Vector2(350, 200));
            confirmnameButton = new Button(content, "Enter", new Vector2(425, 550), Color.Blue, Color.White);
            textenterbox = content.Load<Texture2D>("Images/TextInputBar");
            playername = new Text(content.Load<SpriteFont>("Fonts/buttonFont"),"", new Vector2(375, 400), Color.Blue);
            charAmt = 0;
            nameComplete = false;
            enteredName = new char[8];
            for (int i = 0; i < enteredName.Length; i++)
            {
                enteredName[i] = ' ';
            }
            chooseclasstext = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "Choose Characters Class:", new Vector2(340, 200));
            paladinButton = new Button(content, "Shadow Regulus", new Vector2(425, 400), Color.Blue, Color.White);
            casterButton = new Button(content, "Bezerker", new Vector2(425, 500), Color.Blue, Color.White);
            rogueButton = new Button(content, "Rogue", new Vector2(425, 600), Color.Blue, Color.White);
            storyContinueButton = new Button(content, "Continue", new Vector2(800, 700), Color.Blue, Color.White);
            classComplete = false;
            storytexts = new Texture2D[3];
            storyCount = 0;
            storytexts[0] = content.Load<Texture2D>("Images/Storytext1");
            storytexts[1] = content.Load<Texture2D>("Images/Storytext2");
            storytexts[2] = content.Load<Texture2D>("Images/Storytext3");
            #endregion
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            //if (player.CurrentHP < 1)
            //{
            //    changeScreenDelegate(ScreenState.GameOver);
            //    this.LoadScreenContent(content);
            //}

            if (playerCreated && !inBattle)
            {
              GameplayTurnCheck();
            }
            else
            {
              if (!nameComplete)
              {
                  currentState = Keyboard.GetState();

                  if (currentState.IsKeyDown(Keys.Back))
                  {
                      keydown = '.';
                  }

                  if (currentState.IsKeyUp(Keys.Back) && keydown == '.' && charAmt >= 0)
                  {
                      keydown = (char)0;
                      enteredName[charAmt] = ' ';
                      if (charAmt != 0)
                          charAmt--;
                  }

                  char tempc = GetCharFromKeyboard(currentState);
                  if (charAmt < 8 && tempc != (char)0)
                  {
                      enteredName[charAmt] = tempc;
                      charAmt++;
                  }
                  enteredName[0] = char.ToUpper(enteredName[0]);
                  playername.ChangeText(new string(enteredName));

                  if (input.CheckMouseRelease(confirmnameButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                  {
                      nameComplete = true;
                  }
              }
              else if (!classComplete)
              {                      
                  player.CreatePlayer(0, "Shadow Regulus");
                  classComplete = true;
                  //if (input.CheckMouseRelease(paladinButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                  //{

                  //}

                  //if (input.CheckMouseRelease(casterButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                  //{
                  //    player.CreatePlayer(1, "Bezerker Cid".ToString());
                  //    classComplete = true;

                  //}

                  //if (input.CheckMouseRelease(rogueButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                  //{
                  //    player.CreatePlayer(2, "Tech Guy".ToString());
                  //    classComplete = true;
                  //}
              }
              else
              {
                  playerCreated = true;
                  //if (input.CheckMouseRelease(storyContinueButton) && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                  //{
                  //    if (storyCount < 2)
                  //    {
                  //        storyCount++;
                  //    }
                  //    else
                  //    {
                          
                  //    }
                  //}
              }
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

        protected override void DrawScreen(SpriteBatch batch, DisplayOrientation displayOrientation)
        {
            background.Draw(batch);
            if (playerCreated && !inBattle)
            {
                DrawGameplayItems(batch);
            }
            else
            {
                DrawPregameStuff(batch);
            }
        }
        
        private void GameplayTurnCheck()
        {
            ++tick;
            if (tick % 80 == 0){// tick< 600 * 20) { 
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
                player.Rest();
                UpdateAll();
            }
            Globals.Mobs.Update(input);
        }

        private void UpdateAll() {
            Globals.map.Update();
            Globals.Mobs.UpdateMobs();
        }

       /* private void DrawBattleItems(SpriteBatch batch)
        {
            attackButton.Draw(batch);
            magicButton.Draw(batch);
            potionButton.Draw(batch);
            if (bossBattle)
            {
                boss.Draw(batch);
            }
            else
            {
                minion.Draw(batch);
            }
            HUDPlayerInfo.Draw(batch);
        }*/

        private void DrawGameplayItems(SpriteBatch batch)
        {
            Globals.map.Draw(batch);
            Globals.Mobs.Draw(batch);
            player.Draw(batch);
            attackButton.Draw(batch);

            skipButton.Draw(batch);
            HUDPlayerInfo.Draw(batch);
            leftButton.Draw(batch);
            rightButton.Draw(batch);
            upButton.Draw(batch);
            downButton.Draw(batch);
        }

        private void DrawPregameStuff(SpriteBatch batch)
        {

            if (!nameComplete)
            {
                enterplayernameText.Draw(batch);
                confirmnameButton.Draw(batch);
                batch.Draw(textenterbox, new Rectangle(375, 400, textenterbox.Width, textenterbox.Height), Color.White);
                playername.Draw(batch);
            }
            else if (!classComplete)
            {
                chooseclasstext.Draw(batch);
                paladinButton.Draw(batch);
                casterButton.Draw(batch);
                rogueButton.Draw(batch);
            }
            else
            {
                batch.Draw(storytexts[storyCount], new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
                storyContinueButton.Draw(batch);
            }
        }
    }
}
