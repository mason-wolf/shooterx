using System.Linq;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using TiledSharp;

public class Player
{
    public Vector2 Position;
    public Rectangle Bounds;
    private Texture2D _texture;
    private readonly Rectangle[] _southFrames = new Rectangle[3];
    private readonly Rectangle[] _eastFrames = new Rectangle[3];
    private readonly Rectangle[] _westFrames = new Rectangle[3];
    private int _currentFrame;
    private float _frameTimer;
    private const float FRAME_SPEED = 0.12f;
    private string _currentDirection = "south"; 
    private float speed = 50f;
    private bool _canMove = true;

    public Player(ContentManager content, Vector2 startPosition)
    {
        Position = startPosition;
        Bounds = new Rectangle((int)Position.X, (int)Position.Y, 16, 16);

        _texture = content.Load<Texture2D>("shooter");

        for (int i = 0; i < 3; i++)
        {
            _southFrames[i] = new Rectangle(i * 16, 0, 16, 16);
            _eastFrames[i]  = new Rectangle((i + 3) * 16, 0, 16, 16);
            _westFrames[i]  = new Rectangle((i + 6) * 16, 0, 16, 16);
        }
    }

    public void DisableMovement() { _canMove = false; }
    public void EnableMovement() { _canMove = true; }

    public void Update(GameTime gameTime)
    {
        if (!_canMove) return;

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

            // Set direction priority: horizontal > vertical
            if (velocity.X < 0) _currentDirection = "west";
            else if (velocity.X > 0) _currentDirection = "east";
            else if (velocity.Y < 0) _currentDirection = "east"; // north uses east
            else if (velocity.Y > 0) _currentDirection = "south";

            // Animate only when moving
            _frameTimer += delta;
            if (_frameTimer >= FRAME_SPEED)
            {
                _frameTimer -= FRAME_SPEED;
                _currentFrame = (_currentFrame + 1) % 3;
            }
        }
        else
        {
            _currentFrame = 0; // idle = first frame
            _frameTimer = 0;
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

    private bool CheckTileCollision(TmxLayer collisionLayer, int tw, int th)
    {
        int left   = (int)(Position.X / tw);
        int right  = (int)((Position.X + Bounds.Width) / tw);
        int top    = (int)(Position.Y / th);
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

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle[] frames = _currentDirection switch
        {
            "west"  => _westFrames,
            "east"  => _eastFrames, // also used for north
            _       => _southFrames
        };

        spriteBatch.Draw(_texture, Bounds, frames[_currentFrame], Color.White);
    }
}