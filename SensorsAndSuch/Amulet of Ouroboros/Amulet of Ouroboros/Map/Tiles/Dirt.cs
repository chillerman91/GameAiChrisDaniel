using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Shapes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Amulet_of_Ouroboros.Maps
{
    public class Dirt: BaseTile
    {

        public Dirt(Vector2 GridPos)
            :base ("Tiles/dirt", GridPos)
        {
        }
    }
}
