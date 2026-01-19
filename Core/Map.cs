
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
    private ItemPickupManager _itemPickupManager;
    public Map(Game game) : base(game) { }

    public void LoadMap(string mapPath, string tilesetPath, Player player)
    {
        _map = new TmxMap(mapPath);
        _tileset = Game.Content.Load<Texture2D>(tilesetPath);
        GameState.CurrentMap = _map;
        _player = player;
        _itemPickupManager = new ItemPickupManager(Game, _map, _player);
    }

    public void SpawnEnemies(Game game)
    {
        Enemies.Clear();
        GameState.Enemies.Clear();
        var objectLayer = _map.ObjectGroups.FirstOrDefault(l => l.Name == "enemy");

        if (objectLayer == null) return;

        foreach (var obj in objectLayer.Objects)
        {
            Vector2 pos = new Vector2((float)obj.X + (float)obj.Width / 2, (float)obj.Y + (float)obj.Height / 2);
            var enemyType = obj.Properties.FirstOrDefault(p => p.Key == "type");
            Enemies.Add(new Enemy(game, pos, _map, enemyType.Value));
        }

        GameState.Enemies = Enemies;
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var enemy in Enemies)
            enemy.Update(gameTime, _player);

        _itemPickupManager.Update(gameTime);

    }

    /// <summary>
    /// Draws map objects. For item pickups, see ItemPickupManager.
    /// </summary>
    /// <param name="spriteBatch"></param>
    private void DrawMapObjects(SpriteBatch spriteBatch)
    {
        foreach (var layer in _map.ObjectGroups)
        {
            foreach (var obj in layer.Objects)
            {
                if (obj.Name == "note")
                {
                    Vector2 pos = new Vector2((float)obj.X, (float)obj.Y);
                    spriteBatch.Draw(TextureManager.NoteTexture, pos, null, Color.White);
                }
            }
        }
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
        {
            if (enemy.Health > 0)
            {
                enemy.Draw(spriteBatch);
            }
        }

        DrawMapObjects(spriteBatch);
        _itemPickupManager.Draw(spriteBatch, gameTime);
    }
}