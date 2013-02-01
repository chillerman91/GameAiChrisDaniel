using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Shapes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Amulet_of_Ouroboros.Texts;
using System;

namespace Amulet_of_Ouroboros.Mobs
{
    public class Snake : BaseMonster
    {
        Text info;
        protected Func<bool> State;

        private static int maxHealthPerLevel = 10;

        public Snake(Vector2 GridPos, int id, int level = 1)
            : base("mobs/snake", GridPos, "Snake" + id, GetRandDir(), 10, 0, id)
        {
            hunger = 0;
            State = this.FindFood;
            exp =  15 * (level - 1);
            type = MonTypes.Snake;
            friendly = MonTypes.None;
            Male = Globals.rand.Next(2) == 0;
            if (!Male)
                color = Color.Violet;
            info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), "" + Level, GridPos, Color.Blue);
        }
        public override void Kill()
        {
            //  visual.Draw();
        }

        public override bool IsGoodMate(BaseMonster mon)
        {
            throw new System.NotImplementedException();
        }

        public override void TakeTurn()
        {
            if (Age <= 7 || hunger <= 0)
                Wonder();
            else
                State();
            Age++;
            hunger++;
            if (exp >= Level * 15)
                levelUp();
            if (Age >= 225)
                Globals.Mobs.KillMonster(id);
        }

        protected bool EvalauteFood(BaseMonster mon)
        {
            if (mon.type == friendly) return false;
            if (mon.GetLevel() >= Level + 1)
            {
                flee();
                return false;
            }
            // if (strength >= mon.health) return true;
            // if (strength >= mon.health) return true;
            return true;
        }

        public bool FindFood()
        {
            BaseMonster target = GetAdj(friendly, with: false);
            if (target != null)
            {
                if (EvalauteFood(target))
                {
                    MoveToward(target.GridPos);
                    if (GridPos + moveDir == target.GridPos)
                        AttackPos(target.GridPos);
                }
                else {
                    moveDir = moveDir.Times(-1);
                    Wonder();
                }
            }
            else
            {
                target = FindMonInDir(moveDir);
                if (target != null)
                {
                    MoveToward(target.GridPos);
                    if (GridPos + moveDir == target.GridPos)
                        AttackPos(target.GridPos);
                    else if (MoveStep() == MoveConflict.Wall)
                        Wonder();
                }
                else
                    Wonder();
            }
            return true;
        }

        public bool Mate()
        {
            Vector2? newPos = Globals.map.FindAtHeightFree((int)GridPos.X, (int)GridPos.Y, 1, 5);
            if (newPos != null)
            {
                health = 0;
                Globals.Mobs.AddMonster(type, (Vector2)newPos);

                return true;

            }
            return false;
        }

        private BaseMonster FindMonInDir(Vector2 dir)
        {
            BaseMonster mon;
            mon = FindMonInDir(GridPos, dir, MonTypes.None);
            if (mon != null) return mon;

            mon = FindMonInDir(GridPos + dir, dir, MonTypes.None);
            if (mon != null) return mon;

            mon = FindMonInDir(GridPos + dir + dir, dir, MonTypes.None);
            if (mon != null) return mon;

            return null;
        }

        protected BaseMonster FindMonInDir(Vector2 BasePos, Vector2 dir, MonTypes type)
        {
            BaseMonster mon;
            mon = Globals.Mobs.GetMobAt(BasePos + dir);
            if (mon != null && mon.type != type) return mon;
            mon = Globals.Mobs.GetMobAt(BasePos + dir + dir.Flip());
            if (mon != null && mon.type != type) return mon;
            mon = Globals.Mobs.GetMobAt(BasePos + dir + dir.Flip().Times(-1));
            if (mon != null && mon.type != type) return mon;
            return null;
        }

        private void levelUp() {
            exp -= Level * 15;
            Level += 1;
            strength += 5;
            MaxHealth = Level * 10;
            if (Level >= 5)
            {
                for (int i = 0; i < Math.Min(4, Level - 4); i++)
                    Mate();
            }
        }

        protected bool Wonder()
        {
            int i = Globals.rand.Next(10);
            MoveConflict currConflicct;
            if (i <= 8)
                currConflicct = MoveStep();
            else
            {
                Vector2 tryVec = GetRandDir();
                while (tryVec == moveDir.Times(-1))
                    tryVec = GetRandDir();
                moveDir = tryVec;
                currConflicct = MoveStep();
            }
            if (currConflicct != MoveConflict.None)
            {
                moveDir = GetRandDir();
                currConflicct = MoveStep();
            }


            return true;
        }

        protected bool AttackState()
        {

            MoveConflict currConflicct;
            if (health <= 5 || !SomeOneInDir(moveDir))
                State = Wonder;
            currConflicct = MoveStep();

            if (currConflicct == MoveConflict.Monster)
            {
                AttackPos(GridPos);
            }
            else { State = Wonder; }
            return true;
        }

        protected void AttackPos(Vector2 attackPos)
        {
            BaseMonster mon = Globals.Mobs.GetMobAt(attackPos);
            int nutVal = mon.DoDamage(strength, ref exp);
            if (nutVal != 0)
            {
                health += nutVal;
                health = Math.Min(health, MaxHealth);

                hunger -= nutVal;
                //hunger = Math.Min(0, hunger);
            }
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
            return MoveConflict.Wall;
        }

        protected bool FleeState()
        {
            moveDir = moveDir.Times(-1);
            State = Wonder;
            //Wonder();
            return true;
        }

        protected bool SomeOneInDir(Vector2 dir)
        {
            Vector2 BasePos = GridPos;
            bool forward = Globals.Mobs.GetMobAt(BasePos + dir) != null ||
                Globals.Mobs.GetMobAt(BasePos + dir + dir) != null ||
                Globals.Mobs.GetMobAt(BasePos + dir + dir + dir) != null;

            BasePos = GridPos + dir.Flip();
            bool left = Globals.Mobs.GetMobAt(BasePos + dir) != null ||
                Globals.Mobs.GetMobAt(BasePos + dir + dir) != null ||
                Globals.Mobs.GetMobAt(BasePos + dir + dir + dir) != null;

            BasePos = GridPos + dir.Flip().Times(-1);
            bool right = Globals.Mobs.GetMobAt(BasePos + dir) != null ||
                Globals.Mobs.GetMobAt(BasePos + dir + dir) != null ||
                Globals.Mobs.GetMobAt(BasePos + dir + dir + dir) != null;
            return forward || left || right;
        }

        public override void Draw(SpriteBatch batch)
        {

            CurrentGridPos = CurrentGridPos.Times(0.95) + GridPos.Times(.05);
            batch.Draw(texture, new Rectangle((int)Globals.map.TranslateToPos(CurrentGridPos).X, (int)Globals.map.TranslateToPos(CurrentGridPos).Y, TileWidth, TileHeight), color);
            info.Position = Globals.map.TranslateToPos(CurrentGridPos);
            info.ChangeText("" + Level);
            info.Draw(batch);
        }

       // public override void InformMate(BaseMonster mon) { }
    }
}
