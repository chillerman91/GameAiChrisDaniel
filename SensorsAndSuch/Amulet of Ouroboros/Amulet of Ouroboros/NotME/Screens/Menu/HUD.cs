using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using Amulet_of_Ouroboros.Texts;
using Amulet_of_Ouroboros.Sprites;

namespace Amulet_of_Ouroboros.Screens
{
    class HUDPlayerInfo
    {
        Vector2 BaseLocation = new Vector2(25, 635);
        Texture2D PlayerPieceImage;
        Text Level;
        Text PlayerHealth;
        Text Strength;
        Text exp;

        public HUDPlayerInfo(ContentManager content, Player p)
        {
            PlayerPieceImage = content.Load<Texture2D>("Images/PlayerPiece");
            Level = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), p.Name + "   Lvl: " + p.GetLevel(), BaseLocation + new Vector2(0, 0), Color.Blue);
            PlayerHealth = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "Health: " + p.health + "/", BaseLocation + new Vector2(0, 40), Color.Blue);
            Strength = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "MP: ", BaseLocation + new Vector2(200, 40), Color.Blue);
            exp = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "Pieces: " + 0 , BaseLocation + new Vector2(0, 80), Color.Blue);
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(PlayerPieceImage, new Rectangle(40, 600, PlayerPieceImage.Width, PlayerPieceImage.Height), Color.White);
            Level.Draw(batch);
            PlayerHealth.Draw(batch);
            Strength.Draw(batch);
            exp.Draw(batch);
        }

        public void Update(Player p)
        {
            //Level.ChangeText(p.Name + "   Lvl: " + p.GetLevel());
            //PlayerHealth.ChangeText("Health: " + p.health);
            //Strength.ChangeText("Strength: " + p.GetStrength());
            //exp.ChangeText("Exp: " + p.GetExp() +"/" + p.GetExpForNextLevel());
        }
    }
}
