﻿using System.Collections.Generic;

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

namespace SensorsAndSuch.Mobs
{
    public class Wisker
    {
        #region Datafields

        float WiskerR;
        Texture2D texture;
        Body attatchedTo;
        Color color;
        Color defaultC = Color.White;
        float OffSet;
        private float distance;

        #endregion

        #region Properties

        public float Distance
        {
            get { return distance; }
        }

        #endregion

        public Wisker(Body attatched, float offSet, float WiskerLength)
        {
            texture = Globals.content.Load<Texture2D>("Sensors/Wisker");
            attatchedTo = attatched;
            WiskerR = WiskerLength / .16f;
            OffSet = offSet;
            color = defaultC;
        }
        public Vector2 pt;
        public float Update()
        {
            distance = 1;

            Globals.World.RayCast((fixture, point, normal, fraction) =>
            {
                if (fixture != null && fixture.IsSensor == false && distance > fraction)
                {
                    distance = fraction;
                }
                return 1;
            }
            , attatchedTo.Position, attatchedTo.Position + (attatchedTo.Rotation + OffSet).GetVecFromAng() * WiskerR * .16f);
            System.Diagnostics.Debug.WriteLine(distance);
            int r = (int)(240 * (distance - .3) / .7f) + 10;
            color = new Color(255-r, r, 0);
            return distance;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture,
               Globals.map.TranslateToPos(attatchedTo.Position ), null,
               color, attatchedTo.Rotation + OffSet, new Vector2(texture.Width / 2, texture.Height / 2), WiskerR * distance,
               SpriteEffects.None, 0f);
        }
    }
}