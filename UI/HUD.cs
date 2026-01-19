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


public void Draw(SpriteBatch spriteBatch)
{
    const int width = 200;
    const int height = 25;
    const int margin = 10;
    spriteBatch.Draw(pixel, 
    new Rectangle(margin, margin, width, height), new Color(180, 30, 30));
}
}