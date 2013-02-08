using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Amulet_of_Ouroboros.Mobs;
using Amulet_of_Ouroboros;

namespace Amulet_of_Ouroboros.Maps
{
    public abstract class BaseTile
    {
        protected enum MoveConflict
        {
            Monster,
            Wall,
            None
        }
        public static int TileWidth = 50;
        public static int TileHeight = 50;
        public static int DistBetween = 30;
        public Vector2 GridPos;
        protected Texture2D texture;
        public Color color = Color.White;
        public Color? adjColor = null;
        public SpriteFont font;

        #region for diffrent algorythems
        public bool hit = false;
        public int dist;
        public Vector2 dirToShortest;
        #endregion
        //protected int height;

        protected Vector2 CurrentPos;
        public BaseTile(string tex, Vector2 GridPos)
        {
            font = Globals.content.Load<SpriteFont>("Fonts/buttonFont");
            texture = Globals.content.Load<Texture2D>(tex);
            this.GridPos = GridPos;
            CurrentPos = new Vector2(0, 0);
        }

        public static Vector2 GetRandDir()
        {
            Vector2 newPos;
            int i = Globals.rand.Next(4);
            if (i == 0)
                newPos = new Vector2(-1, 0);
            else if (i == 1)
                newPos = new Vector2(1,0);
            else if (i == 2)
                newPos = new Vector2(0,-1);
            else
                newPos = new Vector2(0, 1);

            return newPos;
        }


        public virtual void Draw(SpriteBatch batch)
        {
            CurrentPos = CurrentPos.Times(0.95) + Globals.map.TranslateToPos(GridPos).Times(.05);
            // batch.Draw(texture, new Rectangle((int)(CurrentPos).X, (int)(CurrentPos).Y, TileWidth, TileHeight), (Color)(adjColor == null ? color : adjColor));
        }

        internal void Update()
        {
            adjColor = null;
        }
    }
}
