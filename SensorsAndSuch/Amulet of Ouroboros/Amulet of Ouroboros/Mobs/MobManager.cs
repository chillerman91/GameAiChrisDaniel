using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Content;
using Amulet_of_Ouroboros.Maps;
using Amulet_of_Ouroboros.Texts;

namespace Amulet_of_Ouroboros.Mobs
{

    public class MobManager
    {
        BaseMonster[] Monsters;
        private static int MaxMonsters = 500;

        Text info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), displayText: "", displayPosition: new Vector2(0, 0), displayColor: Color.White,
                     outlineColor: Color.Black, isTextOutlined: true, alignment: Amulet_of_Ouroboros.Texts.Text.Alignment.None, displayArea: Rectangle.Empty);
        public int Count = 0;
        public MobManager() 
        {
            Monsters = new BaseMonster[MaxMonsters];
        }

        public bool AddPlayer(Player player)
        {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                i++;
            }
            Monsters[i] = player;
            return true;
        }

        public bool AddMonster(BaseMonster.MonTypes monType, Vector2 gridPos, int level = 1)
        {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                i++;
            }
            if (i >= MaxMonsters) return false;
            //if (monType == BaseMonster.MonTypes.Snake)
                //Monsters[i] = new Snake(gridPos, i, level);
            //else if (monType == BaseMonster.MonTypes.Boar)
                Monsters[i] = new BadGuy(gridPos, i);
            return true;
        }

        public bool AddMonster()
        {
            int i = 0;
            while (i < MaxMonsters &&  Monsters[i] != null)
            {
                i++;
            }
            if (i >= MaxMonsters) return false;
            Monsters[i] = new BadGuy(Globals.map.GetRandomFreePos(), i);
            return true;
        }

        public bool FreeFromMobs(Vector2 GridPos)
        {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                if (Monsters[i].SamePos(GridPos)) return false;
                i++;
            }
            return true;
        }

  
        #region Getters and Setters
        
        public BaseMonster GetMobAt(Vector2 GridPos)
        {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                if (Monsters[i].SamePos(GridPos)) return Monsters[i];
                i++;
            }
            return null;
        }
        public BaseMonster GetMobAt(int id)
        {
            return Monsters[id];
        }

        public BaseMonster GetMobAt(float x, float y)
        {
            return GetMobAt(new Vector2(x, y));
        }

        #endregion

        public bool KillMonster(int i)
        {
            Monsters[i].Kill();
            Monsters[i] = null;
            while (i + 1 < MaxMonsters && Monsters[i + 1] != null)
            {
                Monsters[i] = Monsters[i + 1];
                Monsters[i].id = i;

                i++;
            }
            Monsters[i] = null;
            if (i >= MaxMonsters) return false;
            return true;
        }

        public void Draw(SpriteBatch batch) {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                Monsters[i].Draw(batch);
                i++;
            }
            info.Draw(batch);
            Count = i - 1;
        }

        public void UpdateMobs()
        {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                Monsters[i].TakeTurn();
                i++;
            }
        }

        internal void AddMonster(BaseMonster mon1, BaseMonster mon2)
        {
            Vector2? newPos = Globals.map.FindAtHeightFree((int)mon1.GridPos.X, (int)mon1.GridPos.Y, 1, 4);
            if (newPos == null) return;
            AddMonster(mon1.type, (Vector2)newPos);
        }

        internal void Update(Inputs.GameInput input)
        {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                Vector2 tran = Globals.map.TranslateToPos(Monsters[i].GridPos);
                if (input.CheckMouseOver(tran, BaseTile.TileWidth, BaseTile.TileHeight))
                {
                    info.ChangeText(Monsters[i].GetInfo());
                    info.Position = new Vector2(tran.X + BaseTile.TileWidth,tran.Y + BaseTile.TileHeight);
                    return;
                }
                i++;
            }
            info.ChangeText("");
        }
    }
}
