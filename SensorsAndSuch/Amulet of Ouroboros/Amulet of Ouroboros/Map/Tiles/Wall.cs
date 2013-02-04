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
        Body _rectangle;
        private FarseerPhysics.SamplesFramework.Sprite _rectangleSprite;

        public Wall(Vector2 GridPos)
            : base("Tiles/Wall", GridPos)
        {
            //texture = Globals.content.Load<Texture2D>(tex);
            this.GridPos = GridPos/4f;

            _rectangle = BodyFactory.CreateRectangle(Globals.World, width: TileWidth/100f, height: TileHeight/100f, density: 1f);
            _rectangleSprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(_rectangle.FixtureList[0].Shape,
                                                                    MaterialType.Squares,
                                                                    Color.Blue, 1f));

            //_rectangle.BodyType = BodyType.Dynamic;
            int i = 0;
            _rectangle.Position = this.GridPos; //new Vector2(2, -2);//-13.0f + 1.282f * i);
            _rectangle.Friction = 0.75f;
        }

        public override void Draw(SpriteBatch batch)
        {
            // CurrentPos = CurrentPos.Times(0) + Globals.map.TranslateToPos(GridPos); // .Times(.05);
            // batch.Draw(texture, new Rectangle((int)(CurrentPos).X, (int)(CurrentPos).Y, TileWidth, TileHeight), (Color)(adjColor == null ? color : adjColor));
            batch.Draw(_rectangleSprite.Texture,
                   _rectangle.Position * 100, null,
                   Color.White, _rectangle.Rotation, _rectangleSprite.Origin, 1f,
                   SpriteEffects.None, 0f);
        }
    }
}
