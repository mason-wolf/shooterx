using System.Linq;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using TiledSharp;
using System.Collections.Generic;

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
    private List<Projectile> projectiles = new List<Projectile>();
    private float shootCooldown = 0.3f;
    private float shootTimer = 0;
    Game _game;

    public Player(Game game, Vector2 startPosition)
    {
        _game = game;
        Position = startPosition;
        Bounds = new Rectangle((int)Position.X, (int)Position.Y, 16, 16);

        _texture = game.Content.Load<Texture2D>("shooter");
        for (int i = 0; i < 3; i++)
        {
            _southFrames[i] = new Rectangle(i * 16, 0, 16, 16);
            _eastFrames[i] = new Rectangle((i + 3) * 16, 0, 16, 16);
            _westFrames[i] = new Rectangle((i + 6) * 16, 0, 16, 16);
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

            if (velocity.Y < 0) _currentDirection = "north";
            else if (velocity.X < 0) _currentDirection = "west";
            else if (velocity.X > 0) _currentDirection = "east";
            else _currentDirection = "south";

            _frameTimer += delta;
            if (_frameTimer >= FRAME_SPEED)
            {
                _frameTimer -= FRAME_SPEED;
                _currentFrame = (_currentFrame + 1) % 3;
            }
        }
        else
        {
            _currentFrame = 0;
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
            HandleShooting(gameTime, _game);
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

        HandleShooting(gameTime, _game);
    }

    public void HandleShooting(GameTime gameTime, Game game)
    {
        shootTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        var keyboard = Keyboard.GetState();

        Vector2 dir = Vector2.Zero;
        if (keyboard.IsKeyDown(Keys.W)) dir.Y -= 1;
        if (keyboard.IsKeyDown(Keys.S)) dir.Y += 1;
        if (keyboard.IsKeyDown(Keys.A)) dir.X -= 1;
        if (keyboard.IsKeyDown(Keys.D)) dir.X += 1;

        if (dir == Vector2.Zero)
        {
            switch (_currentDirection)
            {
                case "north": dir = new Vector2(0, -1); break;
                case "south": dir = new Vector2(0, 1); break;
                case "west": dir = new Vector2(-1, 0); break;
                case "east": dir = new Vector2(1, 0); break;
            }
        }

        var mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed && shootTimer <= 0)
        {
            dir.Normalize();
            projectiles.Add(new Projectile(game, Position + new Vector2(8, 8), dir));
            shootTimer = shootCooldown;
        }

        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            projectiles[i].Update(gameTime);
            if (!projectiles[i].Active) projectiles.RemoveAt(i);
        }
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

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle[] frames = _currentDirection switch
        {
            "west" => _westFrames,
            "east" => _eastFrames,
            _ => _southFrames
        };

        spriteBatch.Draw(_texture, Bounds, frames[_currentFrame], Color.White);
        foreach (var p in projectiles) p.Draw(spriteBatch);
    }
}