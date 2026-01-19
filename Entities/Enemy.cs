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
    private Texture2D texture;
    private float speed = 30f;
    private List<Vector2> path = new List<Vector2>();
    private int pathIndex = 0;
    private float updateTimer = 0f;
    private const float PATH_UPDATE_INTERVAL = 2f;
    public TmxMap Map;
    private List<Projectile> projectiles = new List<Projectile>();
    private float shootTimer = 0f;
    private float shootCooldown = 2f;
    Game _game;
    public Enemy(Game game, Vector2 startPosition, TmxMap map)
    {
        _game = game;
        Position = startPosition;
        Bounds = new Rectangle((int)Position.X, (int)Position.Y, 16, 16);
        texture = new Texture2D(game.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.Red });
        Map = map;
    }
    public void Update(GameTime gameTime, Player player)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        updateTimer += delta;

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
                16, 16);

            if (projBounds.Intersects(player.Bounds))
            {
                player.Health -= 10;
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
        spriteBatch.Draw(texture, Bounds, Color.White);
        foreach (var p in projectiles) p.Draw(spriteBatch);
    }
}