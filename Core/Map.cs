using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

public class Map : Scene
{
    private TmxMap _map;
    private Texture2D _tileset;
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
    private Player _player;

    public Map(Game game) : base(game) { }

    public void LoadMap(string mapPath, string tilesetPath, Player player)
    {
        _map = new TmxMap(mapPath);
        _tileset = Game.Content.Load<Texture2D>(tilesetPath);
        GameState.CurrentMap = _map;
        _player = player;
    }

    public void SpawnEnemies(GraphicsDevice graphicsDevice)
    {
        Random rand = new Random();
        int attempts = 0;
        const float minDistFromPlayer = 80f;
        const float minDistBetweenEnemies = 128f;

        while (Enemies.Count < 10 && attempts < 1000)
        {
            int tx = (int)(_player.Position.X / _map.TileWidth) + rand.Next(-30, 31);
            int ty = (int)(_player.Position.Y / _map.TileHeight) + rand.Next(-30, 31);

            if (tx < 0 || tx >= _map.Width || ty < 0 || ty >= _map.Height)
            { attempts++; continue; }

            Vector2 candidatePos = new Vector2(tx * _map.TileWidth + 8, ty * _map.TileHeight + 8);

            if (Vector2.Distance(candidatePos, _player.Position) < minDistFromPlayer)
            { attempts++; continue; }

            bool tooClose = Enemies.Any(e => Vector2.Distance(candidatePos, e.Position) < minDistBetweenEnemies);
            if (tooClose) { attempts++; continue; }

            var collisionLayer = _map.Layers.FirstOrDefault(l => l.Name == "collision");
            var tile = collisionLayer?.Tiles[ty * _map.Width + tx];
            if (tile?.Gid == 0)
            {
                Enemies.Add(new Enemy(graphicsDevice, candidatePos, _map));
            }
            attempts++;
        }
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var enemy in Enemies)
            enemy.Update(gameTime, _player.Position);

    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        foreach (TmxLayer layer in _map.Layers)
        {
            if (!layer.Visible) continue;
            for (int y = 0; y < _map.Height; y++)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    TmxLayerTile tile = layer.Tiles[y * _map.Width + x];
                    if (tile.Gid == 0) continue;
                    TmxTileset tileset = _map.Tilesets[0];
                    int tw = _map.TileWidth;
                    int th = _map.TileHeight;
                    int col = (int)((tile.Gid - 1) % tileset.Columns);
                    int row = (int)((tile.Gid - 1) / tileset.Columns);
                    Rectangle source = new Rectangle(col * tw, row * th, tw, th);
                    Vector2 pos = new Vector2(x * tw, y * th);
                    spriteBatch.Draw(_tileset, pos, source, Color.White);
                }
            }
        }

        foreach (var enemy in Enemies)
            enemy.Draw(spriteBatch);
    }
}