
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledSharp;

public class Map : Scene
{
    private TmxMap _map;
    private Texture2D _tileset;
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
    private Player _player;
    private ItemPickupManager _itemPickupManager;
    private List<InteractableObject> _interactables = new List<InteractableObject>();
    private List<Collidable> _collidables = new List<Collidable>();
    Game _game;
    public Map(Game game) : base(game)
    {
        _game = game;
    }

    public void LoadMap(string mapPath, string tilesetPath, Player player)
    {
        _map = new TmxMap(mapPath);
        _tileset = Game.Content.Load<Texture2D>(tilesetPath);
        GameState.CurrentMap = _map;
        _player = player;
        _itemPickupManager = new ItemPickupManager(Game, _map, _player);
        LoadInteractables();
        LoadCollidables();
    }

    public void LoadCollidables()
    {
        _collidables.Clear();

        foreach (var layer in _map.ObjectGroups)
        {
            foreach (var obj in layer.Objects)
            {
                if (obj.Name == "locked-gate")
                {
                    var gateRect = new Rectangle(
                        (int)obj.X,
                        (int)obj.Y,
                        (int)obj.Width,
                        (int)obj.Height
                    );

                    Collidable _lockedGate = new(Game, _player);
                    _lockedGate.Rectangle = gateRect;
                    _lockedGate.Name = "locked-gate";

                    // Track collidable for player collision detection.
                    if (!_collidables.Contains(_lockedGate))
                        _collidables.Add(_lockedGate);
                }
            }
        }
        GameState.Collidables = _collidables;
    }

    public void LoadInteractables()
    {
        foreach (var layer in _map.ObjectGroups)
        {
            foreach (var obj in layer.Objects)
            {
                if (obj.Name == "locked-gate-prompt")
                {

                    LockedGate lockedGate = new(_game, _player);

                    lockedGate.Rectangle = new Rectangle(
                            (int)obj.X,
                            (int)obj.Y,
                            (int)obj.Width,
                            (int)obj.Height
                        );

                    lockedGate.SetInteractionText("(E) Open");

                    _interactables.Add(lockedGate);
                }
            }
        }
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

    private KeyboardState _prevKeyboard;

    public override void Update(GameTime gameTime)
    {

        foreach (var enemy in Enemies)
            enemy.Update(gameTime, _player);

        _itemPickupManager.Update(gameTime);
        var keyboard = Keyboard.GetState();

        foreach (InteractableObject obj in _interactables)
        {
            if (_player.Bounds.Intersects(obj.Rectangle))
            {
                if (keyboard.IsKeyDown(Keys.E) && !_prevKeyboard.IsKeyDown(Keys.E))
                {
                    obj.Interact();
                }
            }
        }

        _prevKeyboard = keyboard;
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

                if (obj.Name == "locked-gate")
                {
                    Vector2 pos = new Vector2((float)obj.X, (float)obj.Y);
                    var lockedGate = TextureManager.LockedGate;

                    foreach (Collidable collidable in GameState.Collidables)
                    {
                        if ((obj.Name == collidable.Name) && !collidable.Destroyed)
                        {
                            spriteBatch.Draw(lockedGate, pos, null, Color.White);
                        }
                    }
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

        foreach (var obj in _interactables)
        {
            if (_player.Bounds.Intersects(obj.Rectangle) && !obj.Destroyed)
            {
                var pos = new Vector2((float)_player.Position.X, (float)_player.Position.Y - 15);
                spriteBatch.DrawString(GameState.Font, obj.GetInteractionText(), pos, new Color(100, 255, 100), 0f, Vector2.Zero, .3f, SpriteEffects.None, 0f);
            }
            else
            {
                obj.ResetState();
            }
        }

        _itemPickupManager.Draw(spriteBatch, gameTime);
    }
}