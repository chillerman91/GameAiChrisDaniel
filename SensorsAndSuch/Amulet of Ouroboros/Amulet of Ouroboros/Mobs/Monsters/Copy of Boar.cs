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
    public class Boa2r : BaseMonster
    {
        Text info;
        protected Func<bool> State;
        protected Boar TargetMate;
        int aggressionVFear = 0;
        private static int maxHealthPerLevel = 10;
        public Boa2r(Vector2 GridPos, int id, int level = 1)
            : base("mobs/Boar", GridPos, "Boar" + id, GetRandDir(), 10 , 0, id)
        {
            hunger = 0;
            State = this.FindFoodState;
            exp = 10 * (level - 1);
            Age = Level * 10;
            type = MonTypes.Boar;
            friendly = MonTypes.Boar;
            aggressionVFear = Globals.rand.Next(100) - 50;
            Male = Globals.rand.Next(2) == 0;
            if (!Male)
                color = Color.Aqua;
            info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), "" + Level, GridPos, Color.Blue);
        }

        public override void Kill()
        {
            //  visual.Draw();
        }

        public override void TakeTurn()
        {
            if (Age % 8 == 0) DesiredAction();
            State();
            Age++;
            hunger++;
            if (exp >= Level * 10)
                levelUp();
            if (hunger > 80)
                health--;
            if (Age >= 300 || health <= 0)
                Globals.Mobs.KillMonster(id);
        }
        
        private void levelUp() {
            Level += 1;
            strength += 3;
            exp -= Level * 10;
            MaxHealth = Level * 20;
        }

        #region States
        protected bool DesiredAction()
        {
            //string action;
            int eat = aggressionVFear + Level * 10 + hunger;  
            int mate = (Level - 2) * 30;
            int task = Globals.rand.Next(Math.Max(1, eat + mate));
            if (eat > mate)
            {
                State = FindFoodState;
            } 
            else 
            {
               State = FindMateState;
            }
            State = FindMateState;
            //aggressionVs. Fear 0-100
            //level
            //age
            //hunger
            return true;

        }
        List<Vector2> path;
        public bool FindMateState()
        {
            BaseMonster target = GetAdj(type, with: true);
            if (target != null)
            {
                if (IsGoodMate(target) && target.IsGoodMate(this))
                {
                    MoveToward(target.GridPos);
                    if (GridPos + moveDir == target.GridPos)
                        Mate(target);
                }
                else
                {
                    moveDir = moveDir.Times(-1);
                    MoveStep();
                }
            }
            else if (TargetMate != null) 
            {
                MoveToward(TargetMate.GridPos);
                if (GridPos + moveDir == TargetMate.GridPos)
                    Mate(target);
                else if (MoveStep() == MoveConflict.Wall)
                    Wonder();
            }
            else
            {
                target = FindMonInDir(moveDir, type, with: true);
                if (target != null && IsGoodMate(target))
                {
                    MoveToward(target.GridPos);
                    if (GridPos + moveDir == target.GridPos)
                        Mate(target);
                    else if (MoveStep() == MoveConflict.Wall)
                        Wonder();
                }
                else
                {
                    CallMates();
                    Wonder();
                }
            }
            return false;
        }

        public bool FindFoodState()
        {
            BaseMonster target = GetAdj(friendly, with: false);
            if (target != null)
            {
                if (true)
                {
                    MoveToward(target.GridPos);
                    if (GridPos + moveDir == target.GridPos)
                        AttackPos(target.GridPos);
                }
                else {
                    moveDir = moveDir.Times(-1);
                    MoveStep();
                }
            }
            else
            {
                target = FindMonInDir(moveDir, friendly, with: false);
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
            return false;
        }

        protected bool Wonder()
        {
            int i = Globals.rand.Next(10);
            MoveConflict currConflicct;
            if (i <= 7)
                currConflicct = MoveStep();
            else
            {
                Vector2 tryVec = GetRandDir();
                while( tryVec == moveDir.Times(-1))
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

        protected bool FleeState()
        {
            moveDir = moveDir.Times(-1);
            State = Wonder;
            //Wonder();
            return true;
        }
        #endregion

        #region State Helpers
        protected bool EvalauteFood(BaseMonster mon)
        {
            if (mon.type == friendly || mon.GetLevel() >= Level + 1) return false;
            if (strength >= mon.health) return true;
            if (strength >= mon.health) return true;
            return true;
        }

        public override bool IsGoodMate(BaseMonster mon)
        {
            return mon.GetLevel() >= 3 && mon.Male != Male;
        }
        
        public bool Mate(BaseMonster mon)
        {
            Vector2? newPos = Globals.map.FindAtHeightFree((int)GridPos.X, (int)GridPos.Y, 1, 4);
            if (newPos != null)
            {
                Globals.Mobs.AddMonster(type, (Vector2)newPos);
                DesiredAction();
                hunger += 5;
                return true;
            }
            return false;
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

        private BaseMonster FindMonInDir(Vector2 dir, MonTypes type, bool with)
        {
            BaseMonster mon;
            mon = FindMonInDir(GridPos, dir, type, with);
            if (mon != null && ((mon.type == type && with) || (mon.type != type && !with))) return mon;

            mon = FindMonInDir(GridPos + dir, dir, type, with);
            if (mon != null && ((mon.type == type && with) || (mon.type != type && !with))) return mon;

            mon = FindMonInDir(GridPos + dir + dir, dir, type, with);
            if (mon != null && ((mon.type == type && with) || (mon.type != type && !with))) return mon;

            return null;
        }

        protected BaseMonster FindMonInDir(Vector2 BasePos, Vector2 dir, MonTypes type, bool with)
        {
            BaseMonster mon;
            mon = Globals.Mobs.GetMobAt(BasePos + dir);
            if (mon != null && ((mon.type == type && with) || (mon.type != type && !with))) return mon;
            mon = Globals.Mobs.GetMobAt(BasePos + dir + dir.Flip());
            if (mon != null && ((mon.type == type && with) || (mon.type != type && !with))) return mon;
            mon =Globals.Mobs.GetMobAt(BasePos + dir + dir.Flip().Times(-1));
            if (mon != null && ((mon.type == type && with) || (mon.type != type && !with))) return mon;
            return null;

        }

        protected void CallMates() {
            BaseMonster mon;
            for (int i = 0; i <= Globals.Mobs.Count; i++) 
            {
                mon = Globals.Mobs.GetMobAt(i);
                if (mon != null && mon.type == type && Male != mon.Male) {
                    mon.InformMate(this);
                }
            }
        }
        public override void InformMate(BaseMonster mon)
        {
            if (IsGoodMate(mon))
            {
                TargetMate = (Boar) mon;
                State = FindMateState;

            }
        }
        #endregion

        public override void Draw(SpriteBatch batch)
        {
            CurrentGridPos = CurrentGridPos.Times(0.9) + GridPos.Times(.1);
            batch.Draw(texture, new Rectangle((int)Globals.map.TranslateToPos(CurrentGridPos).X, (int)Globals.map.TranslateToPos(CurrentGridPos).Y, TileWidth, TileHeight), color);
            info.Position = Globals.map.TranslateToPos(CurrentGridPos);
            info.ChangeText("" + Level);
            info.Draw(batch);
        }
    }
}
