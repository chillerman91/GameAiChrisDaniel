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
    public class Wisker
    {
        float WiskerR;
        Texture2D texture;
        Body attatchedTo;
        Color color;
        Color defaultC = Color.White;
        float OffSet;
        public Wisker(Body attatched, float offSet, float WiskerLength)
        {
            texture = Globals.content.Load<Texture2D>("Sensors/Wisker");
            attatchedTo = attatched;
            WiskerR = WiskerLength / .16f;
            OffSet = offSet;
            color = defaultC;
        }

        public float Update()
        {
            float ret = 1;

            Globals.World.RayCast((fixture, point, normal, fraction) =>
            {
                if (fixture != null)
                {
                    ret = fraction;
                    return 1;
                }
                return fraction;
            }
            , attatchedTo.Position, attatchedTo.Position + (attatchedTo.Rotation + OffSet).GetVecFromAng() * WiskerR * .16f);
            System.Diagnostics.Debug.WriteLine(ret);
            int r =(int) (240*(ret-.3)/.7f) + 10;
            color = new Color(255-r, r, 0);
            return ret;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture,
               (attatchedTo.Position ) * 100, null,
               color, attatchedTo.Rotation + OffSet, new Vector2(texture.Width / 2, texture.Height / 2), WiskerR,
               SpriteEffects.None, 0f);
        }
    }
}