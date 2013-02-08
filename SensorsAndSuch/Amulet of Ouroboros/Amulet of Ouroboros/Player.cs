using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Amulet_of_Ouroboros.Maps;
using Amulet_of_Ouroboros.Mobs;

namespace Amulet_of_Ouroboros.Sprites
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
        public Player(ContentManager content, Vector2 GridPos)
            : base(GridPos, Color.Wheat, 0, FarseerPhysics.Dynamics.BodyType.Dynamic)
        {
            texture = content.Load<Texture2D>("Images/PlayerPiece");
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
        }

        public override void Draw(SpriteBatch batch)
        {
            DrawText(batch);

            base.Draw(batch);
        }

        public void DrawText(SpriteBatch batch)
        {
            string text = string.Format("Wisker Distances: [0]={0}, [1]={1}, [2]={2}", Wiskers[0].Distance, Wiskers[1].Distance, Wiskers[2].Distance);
            batch.DrawString(font, text, this.CurrentGridPos, Color.AliceBlue);//new Vector2(BaseTile.TileWidth, BaseTile.TileHeight * Globals.map.MapHeight * (float)0.95), Color.AliceBlue);
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
