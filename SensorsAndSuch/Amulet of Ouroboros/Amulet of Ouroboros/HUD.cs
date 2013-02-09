using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using SensorsAndSuch.Texts;
using SensorsAndSuch.Sprites;

namespace SensorsAndSuch.Screens
{
    internal class HUDPlayerInfo
    {
        Vector2 BaseLocation = new Vector2(25, 635);
        Texture2D PlayerPieceImage;
        Text Level;
        Player player;
        Text PlayerHealth;
        Text Strength;
        Text exp;
        string[] values;
        public HUDPlayerInfo(ContentManager content, Player p)
        {
            player = p;
            p.SetThisHUD(this);
        }

        public void Draw(SpriteBatch batch)
        {
            if (values != null)
            {
                string text = string.Format("Wisker Distances: [0]={0}, [1]={1}, [2]={2}", values[0], values[1], values[2]);
                batch.DrawString(player.font, text, new Vector2(50, 200), Color.AliceBlue);
            }
        }

        public void Update(params string[]  val)
        {
            values = val;
        }
    }
}
