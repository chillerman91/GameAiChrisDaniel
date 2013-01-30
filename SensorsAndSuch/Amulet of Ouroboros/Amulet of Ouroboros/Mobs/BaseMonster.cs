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

        public abstract bool IsGoodMate(BaseMonster mon);
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
        public int NutVal = 10;
        protected Vector2 moveDir;
        protected Vector2 CurrentGridPos;


        public BaseMonster(string tex, Vector2 GridPos, string Name, Vector2 moveDir, int NutVal, int Age, int id)
            :base (tex, GridPos)
        {
            this.Name = Name;
            this.moveDir = moveDir;
            this.NutVal = NutVal;
            this.Age = Age;
            this.id = id;
        }

        public virtual string GetInfo()
        {
            return "Strength(" + strength + ")\nHealth(" + health + ")\nAge(" + Age + ")";
        }

        public void flee() { moveDir.Times(-1); }

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
                return NutVal * Level;
            }
            return 0;
        }

        protected bool AttackPos(Vector2 attackPos)
        {
            return AttackMon(Globals.Mobs.GetMobAt(attackPos));
        }

        //returns if the monsters dead
        protected bool AttackMon(BaseMonster mon)
        {
            int nutVal = mon.DoDamage(strength, ref exp);
            
            if (nutVal != 0)
            {
                kills++;
                health += nutVal;
                health = Math.Min(health, MaxHealth);

                hunger -= nutVal;
                hunger = Math.Max(0, hunger);
                return true;
            }
            mon.BeingAttacked(this);
            return false;
        }

        public virtual bool BeingAttacked(BaseMonster mon)
        {
            return true;
        }

        protected MoveConflict MoveStep()
        {
            Vector2 newPos;
            newPos = GridPos + moveDir;
            bool MobFree = Globals.Mobs.FreeFromMobs(newPos);
            if (Globals.map.isFree(newPos) && MobFree)
            {
                GridPos = newPos;
                hunger++;

            }
            else if (!MobFree)
            {
                return MoveConflict.Monster;

            }
            else { return MoveConflict.Wall; }

            return MoveConflict.None;

        }

        public void MoveToward(Vector2 other)
        {
            if (other == null) return;
            Vector2 diff = other - GridPos;
            if (other == GridPos)
            {
                moveDir = GetRandDir();
                return;
            }
            Vector2 xMove = new Vector2(diff.X / Math.Abs(diff.X), 0);
            Vector2 yMove = new Vector2(0, diff.Y / Math.Abs(diff.Y));
            List<BaseTile> XColumn = Globals.map.GetTileColumn(GridPos + xMove);
            List<BaseTile> yColumn = Globals.map.GetTileColumn(GridPos + yMove);
            List<Vector2> options = new List<Vector2>();
            if  (diff.X != 0 && XColumn != null && XColumn.Count == 1)
                options.Add(xMove);
            else if (diff.Y != 0 && yColumn != null && yColumn.Count == 1)
                options.Add(yMove);
            if (options.Count == 0) options.Add(GetRandFreeDir());
            moveDir = options[0];

        }

        public void MoveAway(Vector2 other)
        {
            if (other == null) return;
            Vector2 diff = GridPos - other;
            if (other == GridPos)
            {
                moveDir = GetRandFreeDir();
                return;
            }
            if (Math.Abs(diff.X) > Math.Abs(diff.Y))
            {
                moveDir = new Vector2(diff.X / Math.Abs(diff.X), 0);

            }
            else
                moveDir = new Vector2(0, diff.Y / Math.Abs(diff.Y));
        }
        
        public Vector2 GetRandFreeDir()
        {
            List<Vector2> ret = new List<Vector2>();
            List<Vector2> adjCol = Globals.map.GetAdjGridPos((int)GridPos.X, (int)GridPos.Y);
            foreach(Vector2 vec in adjCol)
            {
                if (CanMoveTo((int)vec.X, (int)vec.Y))
                {
                    ret.Add(vec);
                }
            }
            if (ret.Count == 0) throw new Exception();
            return ret[Globals.rand.Next(ret.Count)];
        }

        public virtual bool CanMoveTo(int X, int Y)
        {
            if (Globals.map.isFree(X, Y))
                return true;
            return false;
        }

        public BaseMonster GetAdj(MonTypes type, bool with)
        {
            BaseMonster mon;
            mon = Globals.Mobs.GetMobAt(GridPos.X + 1, GridPos.Y );
            if (mon != null && ((mon.type == type && with) || (mon.type != type && !with))) return mon;

            mon = Globals.Mobs.GetMobAt(GridPos.X - 1, GridPos.Y );
            if (mon != null && ((mon.type == type && with) || (mon.type != type && !with))) return mon;

            mon = Globals.Mobs.GetMobAt(GridPos.X, GridPos.Y + 1);
            if (mon != null && ((mon.type == type && with) || (mon.type != type && !with))) return mon;

            mon = Globals.Mobs.GetMobAt(GridPos.X, GridPos.Y - 1);
            if (mon != null && ((mon.type == type && with) || (mon.type != type && !with))) return mon;

            return null;
        }

        public virtual bool Listen(BaseMonster mon)
        {
            return false;
        }
    }
}
