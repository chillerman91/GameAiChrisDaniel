using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;

namespace SensorsAndSuch.Maps
{
    public class Wall : BaseTile
    {
        Body rectangle;
        private FarseerPhysics.SamplesFramework.Sprite rectangleSprite;

        public Wall(Vector2 GridPos)
            : base("Tiles/Wall", GridPos)
        {
            //texture = Globals.content.Load<Texture2D>(tex);
            this.GridPos = new Vector2(GridPos.X * TileWidth, GridPos.Y * TileHeight);

            rectangle = BodyFactory.CreateRectangle(Globals.World, width: TileWidth, height: TileHeight, density: 5f);
            rectangleSprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(rectangle.FixtureList[0].Shape,
                                                                    MaterialType.Squares,
                                                                    GetColor(this.GridPos), 1f));
            
            //rectangle.BodyType = BodyType.Dynamic;
            rectangle.Position = this.GridPos; //new Vector2(2, -2);//-13.0f + 1.282f * i);
            rectangle.Friction = 0.75f;
        }

        private Color GetColor() 
        {
            return new Color((int)(Globals.rand.Next(100)), 0, (int)(Globals.rand.Next(100))+ 155);
        }

        private Color GetColor(Vector2 gridPos)
        {
            return new Color(((int)(Globals.rand.Next(10) + (int)gridPos.X * 8)) % 255, (int)(Globals.rand.Next((int)gridPos.Y * 5)) % 255, ((int)(Globals.rand.Next((int)gridPos.Y)) + 155) % 255);
            //return new Color((int)(Globals.rand.Next((int)gridPos.X)), 0, (int)(Globals.rand.Next((int)gridPos.Y)) + 155);
        }

        public override void Draw(SpriteBatch batch)
        {
            // CurrentPos = CurrentPos.Times(0) + Globals.map.TranslateToPos(GridPos); // .Times(.05);
            // batch.Draw(texture, new Rectangle((int)(CurrentPos).X, (int)(CurrentPos).Y, TileWidth, TileHeight), (Color)(adjColor == null ? color : adjColor));
            batch.Draw(rectangleSprite.Texture,
                   Globals.map.TranslateToPos(rectangle.Position), null,
                   Color.White, rectangle.Rotation, rectangleSprite.Origin, 1f,
                   SpriteEffects.None, 0f);
        }

        internal override void Delete()
        {
            base.Delete();
            rectangle.Dispose();
        }
    }
}
