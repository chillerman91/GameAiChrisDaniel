using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SensorsAndSuch.Maps
{
    public class Dirt: BaseTile
    {
        public Dirt(Vector2 GridPos)
            :base ("Tiles/dirt", GridPos)
        {
        }
    }
}
