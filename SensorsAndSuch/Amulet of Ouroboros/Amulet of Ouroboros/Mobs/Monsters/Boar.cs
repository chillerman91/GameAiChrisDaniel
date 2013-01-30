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

//Stone monster: can move walls
//water creature: becomes pools
//magician: teleport: time warp
//visible only when seen
// invisible: seen with dust
namespace Amulet_of_Ouroboros.Mobs
{
    public class Boar : BaseMonster
    {
        Text info;
        protected List<Func<bool>> StateStack;

        protected Func<BaseMonster, bool> DesiredAction;
        protected Func<BaseMonster, bool> EvaluateWant;
        protected BaseMonster target;
        int TimeInState = 0;

        #region changing in mind run;
        protected Boar TargetMate;
        List<Vector2> path;
        #endregion

        #region Static Per creator (run with NN)
        int BaseMatingAge = 3;
        private static int maxHealthPerLevel = 15;
        int aggressionVFear = 0;
        int strengthGrowth = 1;
        #endregion

        #region variables from cross State Set Up


        #endregion

        public Boar(Vector2 GridPos, int id, int level = 5)
            : base("mobs/Boar", GridPos, "Snake" + id, GetRandDir(), 15, 0, id)
        {
            hunger = level * 10;
            StateStack = new List<Func<bool>>();
            StateStack.Add(EvaluateWants);
            Age = level * 10;
            exp = 0;
            strength = 10;
            while (level > 1)
                exp += 10 * (level--);

            type = MonTypes.Boar;
            friendly = MonTypes.Boar;
            aggressionVFear = Globals.rand.Next(50) - 25;
            Male = Globals.rand.Next(2) == 0;

            if (!Male)
            {
                color = Color.Aqua;
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
            while(!StateStack[StateStack.Count - 1]());
            Age++;
            //hunger++;
            TimeInState++;
            if (exp >= Level * 10)
                levelUp();
            if (hunger > 50)
                health--;
            if (Age >= 300 || health <= 0)
                Globals.Mobs.KillMonster(id);
        }
        
        private void levelUp() {
            Level += 1;
            strength += strengthGrowth;
            //NutVal = (int) (NutVal * 1.1);
            exp -= Level * 10;
            MaxHealth = Level * maxHealthPerLevel;
        }
        #region StatStack Helpers
        public void popState() 
        {
            StateStack.RemoveAt(StateStack.Count - 1);
        }
        #endregion

        #region States
        protected bool EvaluateWants()
        {
            int eat = aggressionVFear + Level * 10 + hunger;
            int mate = (Level - 2) * 25;
            path = null;
            if (eat < 20 &&mate < 20)  return true;
            else if (eat > mate)
            {
                TimeInState = 0;
                //EvaluateWant = IsGoodFood;
                //DesiredAction = AttackMon;
                target = null;
                StateStack.Add(FindFoodCS);
            } 
            else 
            {
                TimeInState = 0;
                //EvaluateWant = IsGoodMate;
                //DesiredAction = Mate;
                target = null;
                path = null;
                Globals.map.GetTileColumn((int) GridPos.X, (int) GridPos.Y)[0].adjColor= Color.Beige;
                StateStack.Add(FindMateCS);
            }
            return false;
        }

        public bool AttackAS()
        {
            if (target != null && TimeInState <= 6)
            {
                MoveConflict con;
                BaseMonster mon;
                MoveToward(target.GridPos);
                con = MoveStep();
                if (con == MoveConflict.Monster)
                {
                    mon = Globals.Mobs.GetMobAt(GridPos + moveDir);
                    if (target == mon && AttackMon(mon))
                    {
                        target = null;
                        StateStack.RemoveAt(StateStack.Count - 1);
                        return true;
                    }
                }
                return true;
            }
            StateStack.RemoveAt(StateStack.Count - 1);
            return false;
        }

        public bool FindFoodCS()
        {
            MoveConflict currConflicct;
            BaseMonster mon;
            target = null;
            if (TimeInState > 9) {
                StateStack.RemoveAt(StateStack.Count - 1);
                target = null;
                return false;
            }
            GetSensed(IsItimidating);
            if (target != null)
            {
                TimeInState = 0;
                tempPos = target.GridPos;
                StateStack.Add(FleeAS);
                return false;
            }
            GetSensed(IsGoodFood);
            if (target != null)
            {
                TimeInState = 0;
                StateStack.Add(AttackAS);
                return false;
            }
            int count = 0;
            while (RandomMove() != MoveConflict.None && count++<5);
            return true;
        }

        Vector2 tempPos;
        public bool FleeAS()
        {
            Vector2 StartPos = GridPos;
            if (target != null)
            {
                MoveConflict con;
                BaseMonster mon;
                MoveAway(target.GridPos);
                con = MoveStep();
                
                if (TimeInState % 3 == 0) tempPos = StartPos;
                if (TimeInState > 9)
                {
                    TimeInState = 0;
                    StateStack.RemoveAt(StateStack.Count - 1);
                }

                if (con == MoveConflict.Monster)
                {
                    mon = Globals.Mobs.GetMobAt(GridPos + moveDir);
                    AttackMon(mon);
                    target = null;
                    StateStack.RemoveAt(StateStack.Count - 1);
                }
                else if (con == MoveConflict.Wall)
                {
                    int count = 0;
                    while (RandomMove() != MoveConflict.None && count++ < 5) ;
                }
                return true;
            }
            target = null;
            StateStack.RemoveAt(StateStack.Count - 1);
            return false;
        }

        public bool FindMateCS()
        {
            //Every Few 5 Turns InformMates of your presence set the path of interested mates, females will inform 3 have a larger scream radius and will move less.
            if ((!Male && TimeInState % 3 == 0) || (Male && TimeInState % 5 ==0))
                Globals.map.Scream((int) GridPos.X, (int) GridPos.Y, Male ? 5: 7, this);
            if (TimeInState > 20)
            {
                target = null;
                path = null;
                popState();
                return false;
            }
            if (path != null && path.Count > 3)
            {
                //follow path toward mate // attack mon in way
                moveDir = path[path.Count - 1];

                if (MoveConflict.Monster == MoveStep())
                {
                    BaseMonster mon = Globals.Mobs.GetMobAt(GridPos + moveDir);
                    if (IsGoodMate(mon))
                    {
                        target = null;
                        path = null;
                        popState();
                        Mate(mon);
                    }
                    else
                    {
                        AttackPos(mon.GridPos);
                    }
                }
                else
                {
                    path.RemoveAt(path.Count - 1);
                    if (path.Count == 0) path = null;
                }
                return true;
            }
            else 
            {
                GetSensed(IsGoodMate);
                if (target != null)
                {
                    MoveToward(target.GridPos);
                    MoveConflict con = MoveStep();
                    BaseMonster mon = Globals.Mobs.GetMobAt(GridPos + moveDir);
                    if (con == MoveConflict.Monster)
                    {
                        if (IsGoodMate(mon))
                        {
                            target = null;
                            path = null;
                            popState();
                            Mate(mon);
                        }
                        else
                        {
                            AttackPos(mon.GridPos);
                        }
                    }
                }
                else {
                    int count = 0;
                    while (RandomMove() != MoveConflict.None && count++ < 5) ;
                }
                return true;
            }
        }
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
            if (IsGoodMate(mon) && mon.IsGoodMate(this) && StateStack[StateStack.Count - 1] != FindMateCS && Globals.rand.Next(10) > 5) {
                TimeInState = 15;
                StateStack.Add(FindMateCS);
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


                mon = Globals.Mobs.GetMobAt(GridPos + SideStep + lookDir.Times( X));
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
                mon = Globals.Mobs.GetMobAt(GridPos + lookDir + SideStep + lookDir.Times(-1*X));
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

        protected bool IsItimidating(BaseMonster mon)
        {
            if (mon != null && mon.type != friendly && mon.GetLevel() > Level + 2) 
                return true;
            return false;
        }

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
        
        public bool IsBetterMate(BaseMonster mon)
        {
            if (mon == null && mon.GetLevel() > target.GetLevel()) 
            {
                target = mon;
                return true;
            }
            return false;
        }

        protected MoveConflict RandomMove() 
        {
            int i = Globals.rand.Next(10);
            if (i <= 7)
                return MoveStep();
            else
            {
                moveDir = moveDir.Flip().Times(Globals.rand.Next(2) * 2 - 1);
                return MoveStep();
            }
        }

        public bool Mate(BaseMonster mon)
        {
            if (mon != null)
            {
                Globals.map.GetTileColumn((int)GridPos.X, (int)GridPos.Y)[0].adjColor = Color.Blue;
                Globals.map.GetTileColumn((int)mon.GridPos.X, (int)mon.GridPos.Y)[0].adjColor = Color.Blue;
                Globals.Mobs.AddMonster(this, mon);
                hunger += 20;
                return true;
            }
            return false;
        }

        public override bool BeingAttacked(BaseMonster mon)
        {
            target = mon;
            StateStack.Add(AttackAS);
            return true;
        }

        #endregion

        public override string GetInfo()
        {
            return "Strength(" + strength + ")\nHunger("+hunger+")\nHealth(" + health + ")\nAge(" + Age + ")\nState(" + StateStack[StateStack.Count - 1].Method.Name +")";
        }

        public override void Draw(SpriteBatch batch)
        {
            CurrentGridPos = CurrentGridPos.Times(0.95) + GridPos.Times(.05);
            batch.Draw(texture, new Rectangle((int)Globals.map.TranslateToPos(CurrentGridPos).X + TileWidth / 2, (int)Globals.map.TranslateToPos(CurrentGridPos).Y + TileHeight / 2, TileWidth, TileHeight),
                null, color, TranslateVecToRadians(moveDir), new Vector2(32/2, 32/2), SpriteEffects.None, 0f);
            info.Position = Globals.map.TranslateToPos(CurrentGridPos);

            info.ChangeText("" + Level);
            info.Draw(batch);
        }
    }
}

/*
 * 
        protected bool SearchAreaAS()
        {
            int i = Globals.rand.Next(10);
            MoveConflict currConflicct;
            BaseMonster mon = GetSensed();

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
 * 
 * 
 * 
         public bool FindAndActState()
        {
            if (TimeInState < 10 && target == null)
            {
                StateStack.Add(SearchAreaAS);
            }
            else if (target != null) 
            {
                //check world
                MoveConflict con;
                BaseMonster mon = SenseTouch(5, EvaluateWant);
                if (mon != null)
                {
                    target = mon;
                    StateStack.RemoveAt(StateStack.Count - 1);
                    return true;
                }
                mon = SenseVision(3, EvaluateWant);
                if (mon != null)
                {
                    target = mon;
                    StateStack.RemoveAt(StateStack.Count - 1);
                    return true;
                }
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
               // StateStack.Add(FindNewRegionAS);
            }
            return false;
        }
 
 
 */
