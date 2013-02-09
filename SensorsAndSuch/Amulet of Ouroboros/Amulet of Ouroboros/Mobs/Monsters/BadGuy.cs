using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SensorsAndSuch.Texts;
using System;
using SensorsAndSuch.Maps;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;

//Stone monster: can move walls
//water creature: becomes pools
//magician: teleport: time warp
//visible only when seen
// invisible: seen with dust
namespace SensorsAndSuch.Mobs
{
    public class BadGuy : BaseMonster
    {
        public Body circle;
        private FarseerPhysics.SamplesFramework.Sprite Sprite;

        protected static int wiskerNumber = 3;
        protected Wisker[] Wiskers = new Wisker[wiskerNumber];
        protected CircleSensor CircleSensor;
        public BadGuy(Vector2 GridPos, int id)
            : this (GridPos, Color.ForestGreen, id, BodyType.Dynamic)
        {
        }
        public override void Kill() { }
        public BadGuy(Vector2 GridPos, Color color, int id, BodyType bodType)
            : base("mobs/Boar", GridPos, "Snake" + id, GetRandDir(), 15, 0, id)
        {

            //rectangle = BodyFactory.CreateRectangle(Globals.World, width: TileWidth / 200f, height: TileHeight / 200f, density: 1f);

            circle = BodyFactory.CreateCircle(Globals.World, radius: .14f, density: 1f);
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(circle.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                color, 1f));
            //rectangle.FixtureList[0].IsSensor = true;
            circle.LinearDamping = 1.5f;
            circle.AngularDamping = 3f;
            //new FarseerPhysics.SamplesFramework.Sprite(texture);
            circle.BodyType = bodType;
            int i = 0;
            circle.Position = GridPos * TileWidth / 100f;
            circle.Friction = 0.0f;

            Wiskers[0] = new Wisker(attatched: circle, offSet: 0, WiskerLength: 1f);
            Wiskers[1] = new Wisker(attatched: circle, offSet: (float)Math.PI / 4f, WiskerLength: 1f);
            Wiskers[2] = new Wisker(attatched: circle, offSet: (float)Math.PI / -4f, WiskerLength: 1f);
            CircleSensor = new CircleSensor(attatched: circle, color: Color.Green);
        }

        public override void TakeTurn()
        {
            float ret = Wiskers[0].Update();
            float ret1 = Wiskers[1].Update();
            float ret2 = Wiskers[2].Update();// +(Globals.rand.Next((int)4) - 2) / 10f;
            CircleSensor.Update();
            float range = (1 - ret) * 30 + 2;
            // (Globals.rand.Next((int) range) - range/2) / 500f
            circle.ApplyTorque((float) (ret1 - ret2)/100f);
            if (ret > .32 && ret1 > .45)
            {
                circle.ApplyForce(circle.Rotation.GetVecFromAng() * speed * ret1, circle.Position);
            }
            else if (ret > .32 && ret2 > .45)
            {
                circle.ApplyForce(circle.Rotation.GetVecFromAng() * speed * ret2 * -1, circle.Position);
            }
            else
            {
                 circle.ApplyAngularImpulse((.03f) / 100f);
                 circle.ApplyForce(circle.Rotation.GetVecFromAng() * -2 * speed, circle.Position);
            }
            
        }

        public override void Draw(SpriteBatch batch)
        {            
            Wiskers[0].Draw(batch);
            Wiskers[1].Draw(batch);
            Wiskers[2].Draw(batch);

            CircleSensor.Draw(batch);
            batch.Draw(Sprite.Texture,
                               Globals.map.TranslateToPos(circle.Position), null,
                               Color.White, circle.Rotation, Sprite.Origin, 1f,
                               SpriteEffects.None, 0f);
        }
    }
}