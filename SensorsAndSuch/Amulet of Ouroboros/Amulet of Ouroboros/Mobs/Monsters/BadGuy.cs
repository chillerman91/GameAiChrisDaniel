using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Amulet_of_Ouroboros.Texts;
using System;
using Amulet_of_Ouroboros.Maps;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;

//Stone monster: can move walls
//water creature: becomes pools
//magician: teleport: time warp
//visible only when seen
// invisible: seen with dust
namespace Amulet_of_Ouroboros.Mobs
{
    public class BadGuy : BaseMonster
    {
        Text info;
        protected List<Func<bool>> StateStack;

        int TimeInState = 0;

        #region changing in mind run;
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

        Body _rectangle;
        private FarseerPhysics.SamplesFramework.Sprite _rectangleSprite;

        public BadGuy(Vector2 GridPos, int id, int level = 5)
            : base("mobs/Boar", GridPos, "Snake" + id, GetRandDir(), 15, 0, id)
        {

            _rectangle = BodyFactory.CreateRectangle(Globals.World, width: TileWidth / 200f, height: TileHeight / 200f, density: 1f);
            _rectangleSprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(_rectangle.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.ForestGreen, 1f));
            //new FarseerPhysics.SamplesFramework.Sprite(texture);
            _rectangle.BodyType = BodyType.Dynamic;
            int i = 0;
            _rectangle.Position = GridPos/4f;
            _rectangle.Friction = 0.75f;
            info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), "" + Level, GridPos, Color.Blue);
        }


        public override void Kill()
        {
            //  visual.Draw();
        }

        public override void TakeTurn()
        {
            Dir = Dir.AddAng(10);
            _rectangle.ApplyForce(Dir*speed, _rectangle.Position);
        }
        
        private void levelUp() {
            Level += 1;
            strength += strengthGrowth;
            //NutVal = (int) (NutVal * 1.1);
            exp -= Level * 10;
            MaxHealth = Level * maxHealthPerLevel;
        }

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

        #endregion

        public override string GetInfo()
        {
            return "";// "Strength(" + strength + ")\nHunger(" + hunger + ")\nHealth(" + health + ")\nAge(" + Age + ")\nState(" + StateStack[StateStack.Count - 1].Method.Name + ")";
        }

        public override void Draw(SpriteBatch batch)
        {
            CurrentGridPos = Globals.map.TranslateToPos(_rectangle.Position); ; // CurrentGridPos.Times(0.95) + GridPos.Times(.05);
            //batch.Draw(texture, new Rectangle((int)Globals.map.TranslateToPos(CurrentGridPos).X + TileWidth / 2, (int)Globals.map.TranslateToPos(CurrentGridPos).Y + TileHeight / 2, TileWidth, TileHeight),
            //    null, color, TranslateVecToRadians(moveDir), new Vector2(32/2, 32/2), SpriteEffects.None, 0f);
            info.Position = Globals.map.TranslateToPos(CurrentGridPos);

            batch.Draw(_rectangleSprite.Texture,
                               _rectangle.Position * 100, null,
                               Color.White, _rectangle.Rotation, _rectangleSprite.Origin, 1f,
                               SpriteEffects.None, 0f);
            info.ChangeText("" + Level);
            //info.Draw(batch);
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
