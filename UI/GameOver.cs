using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GameOver : Scene
{
    private float timer = 5f;

    public GameOver(Game game) : base(game) { }

    public override void Update(GameTime gameTime)
    {
        timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (timer <= 0)
        {
            GameState.ChangeScene(new MainMenu(Game));
        }
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        float scale = 3f;
        Vector2 textSize = GameState.Font.MeasureString("Game Over") * scale;
        Vector2 center = new Vector2(
            Game.GraphicsDevice.Viewport.Width / 2f - textSize.X / 2f,
            Game.GraphicsDevice.Viewport.Height / 2f - textSize.Y / 2f);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.DrawString(GameState.Font, "Game Over", center, Color.Red, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        spriteBatch.End();
    }
}