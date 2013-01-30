using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Amulet_of_Ouroboros.Mobs;
using Amulet_of_Ouroboros.Maps;
using Microsoft.Xna.Framework.Content;
using Amulet_of_Ouroboros.Sprites;

namespace Amulet_of_Ouroboros
{
    static class Globals
    {
        public static Random rand = new Random();
        public static MobManager Mobs;
        public static RandomMap map;
        public static ContentManager content;
        public static GraphicsDevice Device;
        public static Player player;
        public static void SetGeneral(ContentManager content, GraphicsDevice device)
        {
            Globals.content = content;
            Globals.Device = device;
        }
        public static void SetLevelSpecific(MobManager mobs, RandomMap map)
        {
            Globals.Mobs = mobs;
            Globals.map = map;
        }
    }
}
