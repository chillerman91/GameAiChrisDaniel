﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SensorsAndSuch.Maps;
using SensorsAndSuch.Mobs;
using SensorsAndSuch.Screens;
using System.Collections.Generic;

namespace SensorsAndSuch.Sprites
{
    public class Player : BadGuy
    {
        public enum MoveOpt
        {
            LEFT,
            RIGHT,
            FORWARD,
            BACK,
            NONE
        }
        HUDPlayerInfo HUD;
        public Player(ContentManager content, Vector2 GridPos)
            : base(GridPos, Color.Purple, 0, FarseerPhysics.Dynamics.BodyType.Dynamic)
        {
        }

        internal void SetThisHUD(HUDPlayerInfo HUD)
        {
            this.HUD = HUD;
        }

        public void TakeTurn(MoveOpt Opt)
        {
            float[] ret = new float[3];
            ret[0] = Wiskers[0].Update();
            ret[1] = Wiskers[1].Update();
            ret[2] = Wiskers[2].Update();
            List<Vector2> circleContent = CircleSensor.Update(this.Dir);
            
            switch (Opt)
            {
                case MoveOpt.LEFT:
                    {
                        if (circle.AngularVelocity < 70)
                            circle.ApplyTorque((.5f) / 100f);
                        this.Dir = circle.Rotation.GetVecFromAng();
                        break;
                    }
                case MoveOpt.RIGHT:
                    {
                        if (circle.AngularVelocity > -70)
                            circle.ApplyTorque((-.5f) / 100f);
                        this.Dir = circle.Rotation.GetVecFromAng();
                        break;
                    }
                case MoveOpt.FORWARD:
                    {
                        circle.ApplyForce(circle.Rotation.GetVecFromAng() * speed * 2, circle.Position);
                        break;
                    }
                case MoveOpt.BACK:
                    {
                        circle.ApplyForce(circle.Rotation.GetVecFromAng() * -1 * speed, circle.Position);
                        break;
                    }
            }

            // Update HUD for each sensor type.
            HUD.UpdateWhiskers(string.Format("{0:F2}", ret[2]), string.Format("{0:F2}", ret[0]), string.Format("{0:F2}", ret[1]));

            string adjacentInfo = "Agents: ";
            int i = 0;
            foreach(Vector2 otherAgent in circleContent)
            {
                i++;
                adjacentInfo += string.Format("{0}) dist: {1:F0}, angle: {2:F0}, ", i, otherAgent.X, otherAgent.Y);
            }
            HUD.UpdateAdjacents(adjacentInfo);
        }

        public override void Draw(SpriteBatch batch)
        {
            SetHUDData(batch);

            base.Draw(batch);
        }

        public void SetHUDData(SpriteBatch batch)
        {
        }

        public void CreatePlayer(int clas, string name)
        {
            Name = name;
        }

        internal void Warp()
        {
            circle.Position = Globals.map.GetRandomFreePos() * TileHeight / 100f;
        }

        internal void Rest()
        {
            health += 1;
        }
    }
}
