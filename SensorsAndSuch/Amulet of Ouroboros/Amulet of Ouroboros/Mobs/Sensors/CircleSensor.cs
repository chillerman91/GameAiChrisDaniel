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
using FarseerPhysics.Dynamics.Contacts;

//Stone monster: can move walls
//water creature: becomes pools
//magician: teleport: time warp
//visible only when seen
// invisible: seen with dust
namespace Amulet_of_Ouroboros.Mobs
{
    public class CircleSensor
    {
        protected Body circle;
        private FarseerPhysics.SamplesFramework.Sprite Sprite;

        protected static int wiskerNumber = 3;
        protected Body attatchedTo;
        protected float radius;
        protected Color color = Color.White;
        protected List<Fixture> collided;
        public CircleSensor(Body attatched, Color color, float radius = 1f)
        {
            color.A = 255 / 3;
            circle = BodyFactory.CreateCircle(Globals.World, radius: radius, density: 1f);
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(circle.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.White, 1f));
            circle.FixtureList[0].IsSensor = true;
            circle.FixtureList[0].OnCollision += CollisionHandler;

            circle.FixtureList[0].OnSeparation += SeperationHandler;
            collided = new List<Fixture>();
            //circle.FixtureList[0].OnSeparation += this.GetCollisionHandler;

            circle.Position = attatched.Position;
            attatchedTo = attatched;
            this.radius = radius;
        }

        public bool CollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact) {
            if (!collided.Contains(fixtureB) && !fixtureB.Body.Equals(attatchedTo))
                collided.Add(fixtureB);
            return true;
        }

        public void SeperationHandler(Fixture fixtureA, Fixture fixtureB)
        {
            //if (!collided.Contains(fixtureB) && !fixtureB.Body.Equals(attatchedTo))
            collided.Remove(fixtureB);
        }
        public float GetClosestCollider() 
        {
            float ret = int.MaxValue;
            float  distSqrd = int.MaxValue;
            foreach(Fixture col in collided)
            {
                Vector2 temp = col.Body.Position - circle.Position;
                distSqrd = (temp).LengthSquared();
                if (ret > distSqrd)
                {
                    ret = distSqrd;
                }
            }
            return ret;
        }
        public void Update()
        {
            float distSqrd = GetClosestCollider();
            //color = Color.White;

            if (distSqrd != int.MaxValue)
            {
                float x = (distSqrd / radius);//(radius * radius));
                color = new Color(255- (int)(x * 255), 0, 0, 255 / 3);
            } 
            else
                color = new Color(0, 0, 0, 255 / 3);
            
            circle.Position = attatchedTo.Position;
        }

        public void Draw(SpriteBatch batch)
        {            
            batch.Draw(Sprite.Texture,
                               Globals.map.TranslateToPos(circle.Position), null,
                               color, circle.Rotation, Sprite.Origin, 1f,
                               SpriteEffects.None, 0f);
        }
    }
}