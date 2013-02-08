using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Amulet_of_Ouroboros.Maps;
using System;

namespace Amulet_of_Ouroboros.Mobs
{
    public abstract class BaseMonster : BaseTile
    {
        //Neaural Var;
        //Lifspan
        //NutVal
        //seeable
        //desires
        public enum MonTypes
        {
            Snake, Boar, Player, None
        }

        public abstract void Kill();
        public abstract void TakeTurn();
        public abstract void Draw(SpriteBatch batch);

        #region running states
        protected int Level = 1;
        protected int exp = 0;
        public int health = 20;
        protected int strength = 5;
        protected int hunger;
        public int Age = 0;
        public int kills = 0;
        #endregion

        #region Set At Level
        public string Name;
        public MonTypes type;
        public MonTypes friendly;
        public bool Male;
        public int id;
        #endregion

        public int MaxHealth = 20;
        //public int NutVal = 10;
        protected float speed = .15f;
        protected Vector2 Dir;
        protected Vector2 CurrentGridPos;

        public BaseMonster(string tex, Vector2 GridPos, string Name, Vector2 moveDir, int NutVal, int Age, int id)
            :base (tex, GridPos)
        {
            this.Name = Name;
            this.Dir = moveDir;
            this.Age = Age;
            this.id = id;
        }

        public virtual string GetInfo()
        {
            return "Strength(" + strength + ")\nHealth(" + health + ")\nAge(" + Age + ")";
        }

        public void flee() { Dir.Times(-1); }

        public bool SamePos(Vector2 otherPos)
        {
            return otherPos.VEquals(GridPos);
        }

        public int GetLevel(){return Level;}

        public int DoDamage(int dam, ref int exp)
        {
            Globals.map.GetTileColumn((int)GridPos.X, (int)GridPos.Y)[0].adjColor = Color.Red;
            if (health <= dam)
                exp += health;
            else
                exp += dam;

            health -= dam;
            if (health <= 0) {
                exp += dam * 2;
                Globals.Mobs.KillMonster(id);
                return 1 * Level;
            }
            return 0;
        }

        protected bool AttackPos(Vector2 attackPos)
        {
            return true; // AttackMon(Globals.Mobs.GetMobAt(attackPos));
        }

        public virtual bool Listen(BaseMonster mon)
        {
            return false;
        }
    }
}
