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
        protected Body circle;
        private FarseerPhysics.SamplesFramework.Sprite Sprite;

        protected static int wiskerNumber = 3;
        protected Wisker[] Wiskers = new Wisker[wiskerNumber];

        public BadGuy(Vector2 GridPos, int id)
            : this (GridPos, Color.ForestGreen, id, BodyType.Dynamic)
        {
        }
        public override void Kill() { }
        public BadGuy(Vector2 GridPos, Color color, int id, BodyType bodType)
            : base("mobs/Boar", GridPos, "Snake" + id, GetRandDir(), 15, 0, id)
        {

            //_rectangle = BodyFactory.CreateRectangle(Globals.World, width: TileWidth / 200f, height: TileHeight / 200f, density: 1f);

            circle = BodyFactory.CreateCircle(Globals.World, radius: TileWidth / 300f, density: 1f);
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(circle.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                color, 1f));
            //_rectangle.FixtureList[0].IsSensor = true;
            circle.LinearDamping = (float)1.5;
            circle.AngularDamping = (float)5;
            //new FarseerPhysics.SamplesFramework.Sprite(texture);
            circle.BodyType = bodType;
            int i = 0;
            circle.Position = GridPos / 4f;
            circle.Friction = 0.0f;

            Wiskers[0] = new Wisker(attatched: circle, offSet: 0, WiskerLength: .32f);
            Wiskers[1] = new Wisker(attatched: circle, offSet: (float)Math.PI / 4f, WiskerLength: .32f);
            Wiskers[2] = new Wisker(attatched: circle, offSet: (float)Math.PI / -4f, WiskerLength: .32f);
        }

        public override void TakeTurn()
        {
            float ret = Wiskers[0].Update();
            float ret1 = Wiskers[1].Update();
            float ret2 = Wiskers[2].Update();// +(Globals.rand.Next((int)4) - 2) / 10f;

            float range = (1 - ret) * 30 + 2;
            // (Globals.rand.Next((int) range) - range/2) / 500f
            circle.ApplyTorque((float) (ret1 - ret2)/100f);
            if (ret > .32 && (ret1 > .45 || ret2> .45))
            {
                circle.ApplyForce(circle.Rotation.GetVecFromAng() * speed, circle.Position);
            }
            else
            {
                 circle.ApplyAngularImpulse((.1f) / 100f);
                 circle.ApplyForce(circle.Rotation.GetVecFromAng() * -2 * speed, circle.Position);
            }
            
        }

        public override void Draw(SpriteBatch batch)
        {            
            Wiskers[0].Draw(batch);
            Wiskers[1].Draw(batch);
            Wiskers[2].Draw(batch);

            batch.Draw(Sprite.Texture,
                               circle.Position * 100, null,
                               Color.White, circle.Rotation, Sprite.Origin, 1f,
                               SpriteEffects.None, 0f);

            batch.Draw(Sprite.Texture,
                   circle.Position * 100, null,
                   Color.White, circle.Rotation, Sprite.Origin, .1f,
                   SpriteEffects.None, 0f);

        }
    }
}