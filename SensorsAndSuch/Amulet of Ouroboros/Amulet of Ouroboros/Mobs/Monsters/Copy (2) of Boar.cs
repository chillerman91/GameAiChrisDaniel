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
    public class BoarSecondEdition : BaseMonster
    {
        Text info;
        protected List<Func<bool>> StateStack;

        #region changing in mind run;
        protected Boar TargetMate;
        List<Vector2> path;
        #endregion

        #region Static Per creator (run with NN)
        int BaseMatingAge;
        private static int maxHealthPerLevel = 10;
        int aggressionVFear = 0;
        #endregion

        #region variables from cross State Set Up


        #endregion

        public BoarSecondEdition(Vector2 GridPos, int id, int level = 5)
            : base("mobs/Boar", GridPos, "Boar" + id, GetRandDir(), 10 , 0, id)
        {
            hunger = 0;
            StateStack = new List<Func<bool>>();
            StateStack.Add(DesiredAction);
            Age = level * 10;
            exp = 0;
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
            StateStack[StateStack.Count - 1]();
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
            int eat = aggressionVFear + Level * 10 + hunger;  
            int mate = (Level - 2) * 30;
            int task = Globals.rand.Next(Math.Max(1, eat + mate));
            path = null;
            if (eat > mate)
            {
                StateStack.Add(FindFoodState);
            } 
            else 
            {
                StateStack.Add(FindState);
            }
            return true;
        }
        public bool FindState()
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
        /*
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
        */
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
            return mon.type.Equals(type) && mon.GetLevel() >= BaseMatingAge && Level >= BaseMatingAge && mon.Male != Male;
        }
        
        public bool Mate(BaseMonster mon)
        {
            Vector2? newPos = Globals.map.FindAtHeightFree((int)GridPos.X, (int)GridPos.Y, 1, 4);
            if (newPos != null)
            {
                Globals.Mobs.AddMonster(this, mon);
                DesiredAction();
                hunger += 15;
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
            if (IsGoodMate(mon) && (TargetMate == null || (Globals.map.GetDistBetween(TargetMate.GridPos, GridPos)> Globals.map.GetDistBetween(mon.GridPos, GridPos))))
            {
                TargetMate = (Boar) mon;
                if (StateStack[StateStack.Count - 1] != FindState) 
                    StateStack.Add(FindState);

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
