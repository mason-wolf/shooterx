using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class DebugOverlay
{
public static void Draw(SpriteBatch spriteBatch, Vector2 playerPosition)
{
    string text = $"X: {(int)playerPosition.X}  Y: {(int)playerPosition.Y}";
    Vector2 textPos = new Vector2(playerPosition.X, playerPosition.Y - 5);
    spriteBatch.DrawString(GameState.Font, text, textPos, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
}

public static void ShowText(SpriteBatch spriteBatch, Vector2 playerPosition, string text)
{
    Vector2 textPos = new Vector2(playerPosition.X, playerPosition.Y - 5);
    spriteBatch.DrawString(GameState.Font, text, textPos, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
}
}