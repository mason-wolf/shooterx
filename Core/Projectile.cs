using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Projectile
{
    public Vector2 Position;
    public Vector2 Velocity;
    public bool Active = true;
    private Texture2D texture;
    private static Texture2D bulletTexture;

    public Projectile(Game game, Vector2 start, Vector2 dir)
    {
        if (bulletTexture == null)
        {
            bulletTexture = game.Content.Load<Texture2D>("bullet");
        }
        texture = bulletTexture;

        Position = start;
        Velocity = Vector2.Normalize(dir) * 100f;
    }

    public void Update(GameTime gt)
    {
        if (!Active) return;
        Position += Velocity * (float)gt.ElapsedGameTime.TotalSeconds;
        if (Position.X < 0 || Position.X > 1920 || Position.Y < 0 || Position.Y > 1080) Active = false;
    }

    public void Draw(SpriteBatch sb)
    {
        if (!Active) return;
        sb.Draw(texture, Position, null, Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0f);
    }
}