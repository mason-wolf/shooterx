using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledSharp;

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

    private bool CheckTileCollision(TmxLayer collisionLayer, int tw, int th)
    {
        int left = (int)(Position.X / tw);
        int right = (int)((Position.X + Bounds.Width) / tw);
        int top = (int)(Position.Y / th);
        int bottom = (int)((Position.Y + Bounds.Height) / th);

        for (int y = top; y <= bottom; y++)
        {
            for (int x = left; x <= right; x++)
            {
                if (x < 0 || y < 0 || x >= GameState.CurrentMap.Width || y >= GameState.CurrentMap.Height) continue;

                var tile = collisionLayer.Tiles[y * GameState.CurrentMap.Width + x];
                if (tile.Gid == 0) continue;

                Rectangle tileRect = new Rectangle(x * tw, y * th, tw, th);
                if (Bounds.Intersects(tileRect)) return true;
            }
        }
        return false;
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

        float moveX = velocity.X * speed * delta;
        float moveY = velocity.Y * speed * delta;

        var collisionLayer = GameState.CurrentMap.Layers.FirstOrDefault(l => l.Name == "collision");
        if (collisionLayer == null)
        {
            Position.X += moveX;
            Position.Y += moveY;
            Bounds.X = (int)Position.X;
            Bounds.Y = (int)Position.Y;
            return;
        }

        int tw = GameState.CurrentMap.TileWidth;
        int th = GameState.CurrentMap.TileHeight;

        Rectangle oldBounds = Bounds;

        Position.X += moveX;
        Bounds.X = (int)Position.X;
        if (CheckTileCollision(collisionLayer, tw, th))
        {
            Position.X = oldBounds.X;
            Bounds.X = (int)Position.X;
        }

        Position.Y += moveY;
        Bounds.Y = (int)Position.Y;
        if (CheckTileCollision(collisionLayer, tw, th))
        {
            Position.Y = oldBounds.Y;
            Bounds.Y = (int)Position.Y;
        }
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Bounds, Color.White);
    }
}