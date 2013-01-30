using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Shapes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Amulet_of_Ouroboros.Texts;
using System;
using Amulet_of_Ouroboros.Maps;

namespace Amulet_of_Ouroboros.Mobs
{
    public class SnakeSSS : BaseMonster
    {
        Text info;
        protected List<Func<bool>> StateStack;

        protected Func<BaseMonster, bool> DesiredAction;
        protected Func<BaseMonster, bool> EvaluateWant;
        protected BaseMonster target;

        #region changing in mind run;
        List<Vector2> path;
        #endregion

        //public static class StateInfo 
        //{
        //    public int TimeInState = 0;
        //    protected Func<BaseMonster, bool> DesiredAction;
        //    protected Func<BaseMonster, bool> EvaluateWant;
        //}
        #region Genetic Traits
        int hungerMod = 0;
        int MateMod = 0;
        int RestMod = 0;
        #endregion

        #region Static Per creator (run with NN)
        int BaseMatingAge = 3;
        private static int maxHealthPerLevel = 5;
        int strengthGrowth = 3;
        #endregion

        #region variables from cross State Set Up


        #endregion

        public SnakeSSS(Vector2 GridPos, int id, int level = 1)
            : base("mobs/Snake", GridPos, "Snake " + id, GetRandDir(), 10 , 0, id)
        {
            hunger = level * 10;
            StateStack = new List<Func<bool>>();
            StateStack.Add(EvaluateWants);
            Age = level * 10;
            strength = 3;
            exp = 0;
            while (level > 1)
                exp += 10 * (level--);

            type = MonTypes.Snake;
            friendly = MonTypes.Snake;
            hungerMod = Globals.rand.Next(30) - 15;
            MateMod = Globals.rand.Next(30) - 15;
            RestMod = Globals.rand.Next(30) - 15;
            Male = Globals.rand.Next(2) == 0;
            if (!Male)
            {
                color = Color.Violet;
                strength = (int) (strength * 1.5);
            }
            info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), "" + Level, GridPos, Color.Blue);
        }


        public override void Kill()
        {
            //  visual.Draw();
        }

        public override void TakeTurn()
        {
            StateStack[StateStack.Count - 1]();
            Age++;
            hunger++;
            TimeInState++;
            if (exp >= Level * 10)
                levelUp();
            if (hunger > 80)
                health--;
            if (Age >= 300 || health <= 0)
                Globals.Mobs.KillMonster(id);
        }
        
        private void levelUp() {
            Level += 1;
            strength += strengthGrowth;
            exp -= Level * 10;
            NutVal = (int)(NutVal * 1.5);
            MaxHealth = Level * maxHealthPerLevel;
        }

        #region States
        protected bool EvaluateWants()
        {
            int eat = hungerMod + Level * 10 + hunger;
            int mate = MateMod + (Level - 2) * 30;
            path = null;
            if (eat > mate)
            {
                TimeInState = 0;
                EvaluateWant = IsGoodFood;
                DesiredAction = AttackMon;
                target = null;
                StateStack.Add(FindAndActState);
            } 
            else 
            {
                Globals.map.Scream((int)GridPos.X, (int)GridPos.Y, Male ? 5 : 7, this);
                TimeInState = 0;
                EvaluateWant = IsGoodMate;
                DesiredAction = Mate;
                target = null;
                StateStack.Add(FindAndActState);
            }
            return true;
        }
        int TimeInState = 0;

        public bool FindAndActState()
        {
            if (TimeInState > 20) {
                StateStack.RemoveAt(StateStack.Count - 1);
                return false;
            }
            if (TimeInState < 10 && target == null)
            {
                StateStack.Add(SearchAreaAS);
            }
            else if (target != null) 
            {
                //check world
                MoveConflict con;
                BaseMonster mon;
                MoveToward(target.GridPos);
                con = MoveStep();
                if (con == MoveConflict.Monster)
                {
                    mon = Globals.Mobs.GetMobAt(GridPos + moveDir);
                    if (target == mon && DesiredAction(mon))
                    {
                        target = null;
                        StateStack.RemoveAt(StateStack.Count - 1);
                        return true;
                    }
                }
                else if (MoveConflict.Wall == con)
                {
                    target = null;
                    StateStack.RemoveAt(StateStack.Count - 1);
                }
            } 
            else
            {
                path = null;

                StateStack.RemoveAt(StateStack.Count - 1);
//                StateStack.Add(FindNewRegionAS);
            }
            return false;
        }

        public bool FindState2()
        {
            if (path == null && TargetMate != null) 
            { 
                path = Globals.map.GetShortestPath((int) GridPos.X, (int) GridPos.Y, (int) TargetMate.GridPos.X, (int) TargetMate.GridPos.Y, 30,  tileCol => tileCol.Count == 1);
                if (path != null && path.Count == 0) path = null;
            }
            if (path != null)
            {
                moveDir = path[path.Count - 1];

                if (MoveConflict.Monster == MoveStep())
                {
                    BaseMonster mon = Globals.Mobs.GetMobAt(GridPos + moveDir);
                    if (IsGoodMate(mon))
                    {
                        Mate(mon);
                        StateStack.RemoveAt(StateStack.Count - 1);
                    }
                    else if (IsGoodFood(mon))
                        AttackPos(GridPos + moveDir);
                    else
                    {
                        path = null;
                        StateStack.RemoveAt(StateStack.Count - 1);
                    }
                }
                else 
                { 
                    path.RemoveAt(path.Count - 1);
                    if (path.Count == 0) path = null;
                }
            }
            if (!Male)
            {
                CallMates();
                //Wonder();
            }

            return false;
        }

        public bool FindFoodState()
        {
            if (path == null)
            {
                path = Globals.map.GetShortestPathFromType((int)GridPos.X, (int)GridPos.Y, mon => mon.type != this.type, 20, tileCol => tileCol.Count == 1);
                if (path != null && path.Count == 0) path = null;
            }
            if (path != null)
            {
                moveDir = path[path.Count - 1];

                BaseMonster mon = Globals.Mobs.GetMobAt(GridPos + moveDir);
                MoveConflict con = MoveStep();
                if (MoveConflict.Monster == con && IsGoodFood(mon) && AttackPos(GridPos + moveDir))
                {
                    StateStack.RemoveAt(StateStack.Count - 1);
                }
                else if (MoveConflict.Monster == con && !IsGoodFood(mon))
                {
                    StateStack.RemoveAt(StateStack.Count - 1);
                }
                else
                {
                    path.RemoveAt(path.Count - 1);
                    if (path.Count == 0) path = null;
                }
            }
            return false;
        }

        public bool FindNewRegionAS()
        {
            if (path == null)
            {
                Vector2 pos = Globals.map.GetRandomFreePos();
                path = Globals.map.GetShortestPath((int)GridPos.X, (int)GridPos.Y, (int)pos.X, (int)pos.Y, 30, tileCol => tileCol.Count == 1);
                if (path != null && path.Count == 0) path = null;
            }
            if (path != null)
            {
                moveDir = path[path.Count - 1];

                BaseMonster mon = Globals.Mobs.GetMobAt(GridPos + moveDir);
                MoveConflict con = MoveStep();
                if (MoveConflict.None != con)
                {
                    path = null;
                    TimeInState = 0;
                    StateStack.RemoveAt(StateStack.Count - 1);
                    return true;
                }
                else
                {
                    path.RemoveAt(path.Count - 1);
                    if (path.Count == 0) path = null;
                }
            }
            return false;
        }

        protected bool SearchAreaAS()
        {
            int i = Globals.rand.Next(10);
            MoveConflict currConflicct;
            BaseMonster mon = SenseTouch(3, EvaluateWant);
            if (mon != null)
            {
                target = mon;
                StateStack.RemoveAt(StateStack.Count - 1);
                return true;
            }
            mon = SenseVision(3, EvaluateWant);
            if (mon != null) {
                target = mon;
                StateStack.RemoveAt(StateStack.Count - 1);
                return true;
            }

            if (i <= 7)
                currConflicct = MoveStep();
            else
            {
                moveDir = moveDir.Flip().Times(Globals.rand.Next(2) * 2 - 1);
                currConflicct = MoveStep();
            }
            if (currConflicct != MoveConflict.None)
            {
                Vector2 temp = moveDir;
                while (temp == moveDir)
                    temp = GetRandDir();
                moveDir = temp;
                currConflicct = MoveStep();
            }
            return false;
        }
        /*
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
        */
        #endregion

        #region Senses
        protected bool GetSensed(Func<BaseMonster, bool> isWanted)
        {
            BaseMonster mon;
            mon = SenseTouch(3, isWanted);
            if (mon != null)
            {
                target = mon;
                return true;
            }
            mon = SenseVision(3, isWanted);
            if (mon != null)
            {
                target = mon;
                return true;
            }
            return false;
        }

        public override bool Listen(BaseMonster mon)
        {
            if (IsGoodMate(mon) && mon.IsGoodMate(this) && DesiredAction != Mate && Globals.rand.Next(10) > 5)
            {
                TimeInState = 10; 
                EvaluateWant = IsGoodMate;
                DesiredAction = Mate;
                target = null;
                StateStack.Add(FindAndActState);
                path = Globals.map.GetPath((int)mon.GridPos.X, (int)mon.GridPos.Y, (int)GridPos.X, (int)GridPos.Y);
            }
            return IsGoodMate(mon);
        }

        //Currenlty in a square pattern;
        protected BaseMonster SenseVision(int dist, Func<BaseMonster, bool> isWanted)
        {
            BaseMonster mon;
            Vector2 lookDir = moveDir;
            lookDir.Normalize();
            List<BaseTile> column;
            Vector2 SideStep = lookDir.Flip();
            bool hitW1 = false, hitW2 = false, hitW3 = false;
            mon = Globals.Mobs.GetMobAt(GridPos + lookDir);
            if (mon != null && isWanted(mon))
                return mon;
            for (int X = 1; X <= dist; X++)
            {
                mon = Globals.Mobs.GetMobAt(GridPos + lookDir.Times(X));
                if (mon != null && isWanted(mon) && !hitW1)
                    return mon;
                column = Globals.map.GetTileColumn(GridPos + lookDir.Times(X));
                if (column != null && column.Count != 1) hitW1 = true;


                mon = Globals.Mobs.GetMobAt(GridPos + SideStep + lookDir.Times(X));
                if (mon != null && isWanted(mon) && !hitW2)
                    return mon;
                column = Globals.map.GetTileColumn(GridPos + SideStep + lookDir.Times(X));
                if (column != null && column.Count != 1) hitW2 = true;


                mon = Globals.Mobs.GetMobAt(GridPos + SideStep.Times(-1) + lookDir.Times(X));
                if (mon != null && isWanted(mon) && !hitW3)
                    return mon;
                column = Globals.map.GetTileColumn(GridPos + SideStep.Times(-1) + lookDir.Times(X));
                if (column != null && column.Count != 1) hitW3 = true;
            }
            return null;
        }

        protected BaseMonster SenseTouch(int dist, Func<BaseMonster, bool> isWanted)
        {
            BaseMonster mon;
            Vector2 lookDir = moveDir;
            lookDir.Normalize();
            Vector2 SideStep = lookDir.Flip();

            mon = Globals.Mobs.GetMobAt(GridPos + lookDir);
            if (mon != null && isWanted(mon))
                return mon;
            for (int X = 0; X < dist; X++)
            {
                mon = Globals.Mobs.GetMobAt(GridPos + lookDir + SideStep + lookDir.Times(-1 * X));
                if (mon != null && isWanted(mon))
                    return mon;
                mon = Globals.Mobs.GetMobAt(GridPos + lookDir + SideStep.Times(-1) + lookDir.Times(-1 * X));
                if (mon != null && isWanted(mon))
                    return mon;
            }
            return null;
        }
        #endregion

        #region State Helpers
        protected bool IsGoodFood(BaseMonster mon)
        {
            if (mon == null || mon.type == friendly || mon.GetLevel() > Level + 1) return false;
            if (strength >= mon.health) return true;
            if (strength >= mon.health) return true;
            return true;
        }

        public override bool IsGoodMate(BaseMonster mon)
        {
            return mon != null && mon.type.Equals(type) && mon.GetLevel() >= BaseMatingAge && Level >= BaseMatingAge && mon.Male != Male;
        }
        
        public bool Mate(BaseMonster mon)
        {
            if (mon != null)
            {
                Globals.Mobs.AddMonster(this, mon);
                Globals.Mobs.AddMonster(this, mon);
                //EvaluateWants();
                hunger += 15;
                return true;
            }
            return false;
        }
        #endregion
        
        public override string GetInfo()
        {
            return Name + "\nStrength(" + strength + ")\nHealth(" + health + ")\nAge(" + Age + ")\nState(" + DesiredAction + ")";
        }

        public override void Draw(SpriteBatch batch)
        {
            CurrentGridPos = CurrentGridPos.Times(0.9) + GridPos.Times(.1);
            batch.Draw(texture, new Rectangle((int)Globals.map.TranslateToPos(CurrentGridPos).X + TileWidth / 2, (int)Globals.map.TranslateToPos(CurrentGridPos).Y + TileHeight / 2, TileWidth, TileHeight),
                null, color, TranslateVecToRadians(moveDir), new Vector2(32/2, 32/2), SpriteEffects.None, 0f);
            info.Position = Globals.map.TranslateToPos(CurrentGridPos);

            info.ChangeText("" + Level);
            info.Draw(batch);
        }
    }
}
