using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Amulet_of_Ouroboros.Maps;
using Amulet_of_Ouroboros.Mobs;

namespace Amulet_of_Ouroboros.Sprites
{
    public class Player : BaseMonster
    {
        private static int expNeededPerLevel = 10;
        //int BaseMatingAge = 3;
        private static int maxHealthPerLevel = 15;
        int aggressionVFear = 0;
        int strengthGrowth = 2;
        public int Pieces;

        public override void Kill() { }

        public override void TakeTurn() { }

        public Player(ContentManager content)
            : base("Images/PlayerPiece",  Globals.map.GetRandomFreePos(), "player", new Vector2(),  NutVal: 20, Age: 0, id: 0)
        {
            texture = content.Load<Texture2D>("Images/PlayerPiece");
            Pieces = 0;
            strength = 8;
            type = MonTypes.Player;
        }

        public override bool  IsGoodMate(BaseMonster mon)
        {
 	        throw new System.NotImplementedException();
        }
        
        public void CreatePlayer(int clas, string name)
        {
            Name = name;
        }
        #region UnNeeded
        public void UpdatePosition(){        }

       // public override void InformMate(BaseMonster mon) { }

        protected virtual void UpdateSprite(GameTime gameTime) {  }
        #endregion

        public override void Draw(SpriteBatch batch)
        {

            CurrentGridPos = CurrentGridPos.Times(0.95) + GridPos.Times(.05);
            batch.Draw(texture, new Rectangle((int)Globals.map.TranslateToPos(CurrentGridPos).X, (int)Globals.map.TranslateToPos(CurrentGridPos).Y, TileWidth, TileHeight), color);
            DrawSprite(batch);
        }

        protected virtual void DrawSprite(SpriteBatch batch)
        {
        }

        public int Height
        {
            get { return texture.Height; }
        }

        public int Width
        {
            get { return texture.Width; }
        }

        internal bool MakeMove(float X, float Y)
        {
            return MakeMove((int)X, (int)Y);
        }

        internal bool MakeMove(int X, int Y)
        {
            moveDir = new Vector2(X, Y) - GridPos;
            MoveConflict con = MoveStep();
            if (exp > Level * expNeededPerLevel)
                LevelUp();
            if (con == MoveConflict.Wall) return false;
            if (con == MoveConflict.None) return true;
            if (con == MoveConflict.Monster)
            {
                AttackPos(new Vector2(X, Y));
                return true;

            }

            return false;

        }
        private void LevelUp() 
        { 
            MaxHealth += maxHealthPerLevel;
            exp -= Level * expNeededPerLevel;
            strength += strengthGrowth;
            Level++;
        }
        internal void Warp()
        {
            GridPos = Globals.map.GetRandomFreePos();
        }

        internal int GetStrength()
        {
            return strength;
        }

        internal int GetExp()
        {
            return exp;
        }

        internal int GetExpForNextLevel()
        {
            return Level * expNeededPerLevel;
        }

        internal void Rest()
        {
            health += 1;
        }
    }
}
