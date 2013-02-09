using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SensorsAndSuch.Maps;
using SensorsAndSuch.Mobs;
using SensorsAndSuch.Screens;

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

            float ret = Wiskers[0].Update();
            float ret1 = Wiskers[1].Update();
            float ret2 = Wiskers[2].Update();
            CircleSensor.Update();
            switch (Opt)
            {
                case MoveOpt.LEFT:
                    {
                        if (circle.AngularVelocity < 70)
                            circle.ApplyTorque((.5f) / 100f);
                        break;
                    }
                case MoveOpt.RIGHT:
                    {
                        if (circle.AngularVelocity > -70)
                            circle.ApplyTorque((-.5f) / 100f);
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
            HUD.Update(string.Format("{0:F2}", ret2), string.Format("{0:F2}", ret), string.Format("{0:F2}", ret1));
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
