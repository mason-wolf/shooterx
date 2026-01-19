using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

public class Enemy : Entity
{
    public Vector2 Position;
    public Rectangle Bounds;
    private string _type;
    private Texture2D _texture;
    private float speed = 30f;
    private List<Vector2> path = new List<Vector2>();
    private int pathIndex = 0;
    private float updateTimer = 0f;
    private const float PATH_UPDATE_INTERVAL = 2f;
    public TmxMap Map;
    private List<Projectile> projectiles = new List<Projectile>();
    private float shootTimer = 0f;
    private float shootCooldown = 3f;
    Game _game;
    private readonly Rectangle[] _southFrames = new Rectangle[3];
    private readonly Rectangle[] _eastFrames = new Rectangle[3];
    private readonly Rectangle[] _westFrames = new Rectangle[3];
    private string _currentDirection = "south";
    private int _currentFrame;
    private float _frameTimer;
    private const float FRAME_SPEED = 0.12f;
    public Enemy(Game game, Vector2 startPosition, TmxMap map, string type)
    {
        _game = game;
        Position = startPosition;
        Bounds = new Rectangle((int)Position.X, (int)Position.Y, 16, 16);
        _texture = game.Content.Load<Texture2D>("thug-1");
        for (int i = 0; i < 3; i++)
        {
            _southFrames[i] = new Rectangle(i * 16, 0, 16, 16);
            _eastFrames[i] = new Rectangle((i + 3) * 16, 0, 16, 16);
            _westFrames[i] = new Rectangle((i + 6) * 16, 0, 16, 16);
        }
        Map = map;
        _type = type;
    }
public void Update(GameTime gameTime, Player player)
{
    float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
    updateTimer += delta;

    Vector2 previousPosition = Position;

    if (updateTimer >= PATH_UPDATE_INTERVAL || pathIndex >= path.Count)
    {
        path = AStar(player.Position);
        pathIndex = 0;
        updateTimer = 0f;
    }

    if (pathIndex < path.Count)
    {
        Vector2 target = path[pathIndex];
        Vector2 dir = Vector2.Normalize(target - Position);
        Position += dir * speed * delta;
        Bounds.X = (int)Position.X;
        Bounds.Y = (int)Position.Y;

        if (Vector2.Distance(Position, target) < 8f)
            pathIndex++;
    }

    // Separation to prevent stacking
    Vector2 separation = Vector2.Zero;
    int count = 0;

    foreach (var other in GameState.Enemies)
    {
        if (other != this && Vector2.Distance(Position, other.Position) < 40f)
        {
            Vector2 diff = Position - other.Position;
            separation += diff != Vector2.Zero ? Vector2.Normalize(diff) / diff.Length() : Vector2.Zero;
            count++;
        }
    }

    if (count > 0)
    {
        separation /= count;
        separation.Normalize();
        Position += separation * speed * delta * 0.5f;
        Bounds.X = (int)Position.X;
        Bounds.Y = (int)Position.Y;
    }

    // Direction & animation
    Vector2 moveDir = Position - previousPosition;
    if (moveDir.LengthSquared() > 0.01f)
    {
        moveDir.Normalize();
        if (Math.Abs(moveDir.Y) > Math.Abs(moveDir.X))
            _currentDirection = moveDir.Y < 0 ? "north" : "south";
        else
            _currentDirection = moveDir.X < 0 ? "west" : "east";

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

    shootTimer -= delta;

    if (shootTimer <= 0 && Health > 0)
    {
        Vector2 dir = Vector2.Normalize(player.Position - Position);
        projectiles.Add(new Projectile(_game, Position + new Vector2(8, 8), dir));
        shootTimer = shootCooldown;
    }

    for (int i = projectiles.Count - 1; i >= 0; i--)
    {
        projectiles[i].Update(gameTime);

        Rectangle projBounds = new Rectangle(
            (int)projectiles[i].Position.X - 4,
            (int)projectiles[i].Position.Y - 4,
            8, 8);

        if (projBounds.Intersects(player.Bounds))
        {
            player.Health -= 5;
            projectiles.RemoveAt(i);
        }
        else if (!projectiles[i].Active)
        {
            projectiles.RemoveAt(i);
        }
    }
}
    private List<Vector2> AStar(Vector2 goal)
    {
        var openSet = new List<(Vector2 pos, float g, float h)>();
        var cameFrom = new Dictionary<Vector2, Vector2>();
        var gScore = new Dictionary<Vector2, float>();
        var collisionLayer = Map.Layers.FirstOrDefault(l => l.Name == "collision");

        Vector2 startTile = new Vector2((int)(Position.X / Map.TileWidth), (int)(Position.Y / Map.TileHeight));
        Vector2 goalTile = new Vector2((int)(goal.X / Map.TileWidth), (int)(goal.Y / Map.TileHeight));

        gScore[startTile] = 0;
        openSet.Add((startTile, 0, Heuristic(startTile, goalTile)));

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(n => n.g + n.h).First();
            openSet.Remove(current);
            Vector2 currTile = current.pos;

            if (currTile == goalTile)
                return ReconstructPath(cameFrom, currTile);

            int[] dirs = { 0, 1, 0, -1, -1, 0, 1, 0, -1, -1, 1, 1, 1, -1, -1, 1 };
            for (int i = 0; i < dirs.Length; i += 2)
            {
                Vector2 neighbor = currTile + new Vector2(dirs[i], dirs[i + 1]);
                if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.X >= Map.Width || neighbor.Y >= Map.Height) continue;

                var tile = collisionLayer.Tiles[(int)(neighbor.Y * Map.Width + neighbor.X)];
                if (tile.Gid != 0) continue;

                float tentG = gScore[currTile] + 1;
                Vector2 nKey = neighbor;

                if (!gScore.ContainsKey(nKey) || tentG < gScore[nKey])
                {
                    cameFrom[nKey] = currTile;
                    gScore[nKey] = tentG;
                    float h = Heuristic(neighbor, goalTile);
                    openSet.Add((neighbor, tentG, h));
                }
            }
        }
        return new List<Vector2> { startTile };
    }

    private float Heuristic(Vector2 a, Vector2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    private List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
    {
        List<Vector2> path = new List<Vector2> { current * new Vector2(Map.TileWidth, Map.TileHeight) + new Vector2(Map.TileWidth / 2, Map.TileHeight / 2) };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current * new Vector2(Map.TileWidth, Map.TileHeight) + new Vector2(Map.TileWidth / 2, Map.TileHeight / 2));
        }
        return path;
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