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
        string[] whiskerValues;
        string adjascentValues;

        public HUDPlayerInfo(ContentManager content, Player p)
        {
            player = p;
            p.SetThisHUD(this);
        }

        public void Draw(SpriteBatch batch)
        {
            if (whiskerValues != null)
            {
                string text = string.Format("Wisker Distances: [0]={0}, [1]={1}, [2]={2}", whiskerValues[0], whiskerValues[1], whiskerValues[2]);
                batch.DrawString(player.font, text, new Vector2(50, 20), Color.AliceBlue);
            }
            if (!string.IsNullOrEmpty(adjascentValues))
            {
                batch.DrawString(player.font, adjascentValues, new Vector2(50, 40), Color.AliceBlue); 
            }
        }

        public void UpdateWhiskers(params string[] val)
        {
            whiskerValues = val;
        }

        public void UpdateAdjacents(string val)
        {
            adjascentValues = val;
        }

    }
}
