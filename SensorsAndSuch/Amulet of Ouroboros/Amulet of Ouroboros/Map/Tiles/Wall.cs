using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;

namespace Amulet_of_Ouroboros.Maps
{
    public class Wall : BaseTile
    {
        /*
        public Wall(Vector2 GridPos)
            :base ("Tiles/Wall", GridPos)
        {
        }
        */
        Body rectangle;
        private FarseerPhysics.SamplesFramework.Sprite rectangleSprite;

        public Wall(Vector2 GridPos)
            : base("Tiles/Wall", GridPos)
        {
            //texture = Globals.content.Load<Texture2D>(tex);
            this.GridPos = GridPos * TileWidth / 100f;

            rectangle = BodyFactory.CreateRectangle(Globals.World, width: TileWidth/100f, height: TileHeight/100f, density: 5f);
            rectangleSprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(rectangle.FixtureList[0].Shape,
                                                                    MaterialType.Squares,
                                                                    Color.Blue, 1f));

            //rectangle.BodyType = BodyType.Dynamic;
            rectangle.Position = this.GridPos; //new Vector2(2, -2);//-13.0f + 1.282f * i);
            rectangle.Friction = 0.75f;
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
    }
}
