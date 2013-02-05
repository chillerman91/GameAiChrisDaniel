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
            : base(GridPos, Color.Wheat, 0)
        {
            texture = content.Load<Texture2D>("Images/PlayerPiece");
        }
        public void TakeTurn(MoveOpt Opt)
        {

            float ret = Wiskers[0].Update();
            float ret1 = Wiskers[1].Update();
            float ret2 = Wiskers[2].Update();
            switch (Opt)
            {
                case MoveOpt.LEFT:
                    {
                        if (circle.AngularVelocity <30)
                            circle.ApplyTorque((.1f) / 100f);
                        break;
                    }
                case MoveOpt.RIGHT:
                    {
                        if (circle.AngularVelocity > -30)
                            circle.ApplyTorque((-.1f) / 100f);
                        break;
                    }
                case MoveOpt.FORWARD:
                    {
                        circle.ApplyForce(circle.Rotation.GetVecFromAng() * speed, circle.Position);
                        break;
                    }
                case MoveOpt.BACK:
                    {
                        circle.ApplyForce(circle.Rotation.GetVecFromAng() * -1 * speed, circle.Position);
                        break;
                    }
            }
        }

        public void CreatePlayer(int clas, string name)
        {
            Name = name;
        }

        internal void Warp()
        {
            GridPos = Globals.map.GetRandomFreePos();
        }

        internal void Rest()
        {
            health += 1;
        }
    }
}
