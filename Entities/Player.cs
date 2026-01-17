using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Player
{
    public Vector2 Position;
    public Rectangle Bounds;
    private Texture2D texture;
    private float speed = 200f;

    public Player(GraphicsDevice graphicsDevice, Vector2 startPosition)
    {
        Position = startPosition;
        Bounds = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 velocity = Vector2.Zero;

        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.A)) velocity.X -= 1;
        if (keyboard.IsKeyDown(Keys.D)) velocity.X += 1;
        if (keyboard.IsKeyDown(Keys.W)) velocity.Y -= 1;
        if (keyboard.IsKeyDown(Keys.S)) velocity.Y += 1;

        if (velocity.LengthSquared() > 0)
        {
            velocity.Normalize();
        }

        Position += velocity * speed * delta;
        Bounds.X = (int)Position.X;
        Bounds.Y = (int)Position.Y;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Bounds, Color.White);
    }
}