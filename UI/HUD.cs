using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class HUD
{
    private Texture2D pixel;
    public HUD(GraphicsDevice graphicsDevice)
    {
        pixel = new Texture2D(graphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

public void Draw(SpriteBatch spriteBatch, Player player)
{
    const int width = 200;
    const int height = 25;
    const int margin = 10;
    const int ammoGap = 15;
    const int ammoIconSize = 24;

    float healthRatio = player.Health / 100f;
    int fillWidth = (int)(width * healthRatio);

    spriteBatch.Draw(pixel, new Rectangle(margin, margin, width, height), new Color(50, 50, 50));

    spriteBatch.Draw(pixel,
        new Rectangle(margin, margin, fillWidth, height),
        new Color(200 - (int)(170 * healthRatio), 30 + (int)(170 * healthRatio), 30));

    int ammoX = margin + width + ammoGap;
    int ammoY = margin + (height - ammoIconSize) / 2;

    spriteBatch.Draw(TextureManager.AmmoTexture, 
        new Rectangle(ammoX, ammoY, ammoIconSize, ammoIconSize), 
        Color.White);


    spriteBatch.DrawString(GameState.Font, player.Ammo.ToString(),
        new Vector2(260, 12),
        Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
}
}