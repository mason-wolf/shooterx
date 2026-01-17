using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;
public class TestScene : Scene
{
    SpriteFont _font;
    TmxMap _map;
    Texture2D _tileset;
    private Player _player;
    private Camera _camera;
    public TestScene(Game game) : base(game) { }

    public override void LoadContent()
    {
        _font = Game.Content.Load<SpriteFont>("Munro");
         _map = new TmxMap("Content/test-map.tmx");
        _tileset = Game.Content.Load<Texture2D>("tileset.png");
        _player = new Player(Game.GraphicsDevice, new Vector2(10, 10));
        _camera = new Camera(Game.GraphicsDevice.Viewport);
    }
    public override void Update(GameTime gameTime)
    {
        _player.Update(gameTime);
        _camera.SetTarget(_player.Position + new Vector2(16,16));
        _camera.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

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
                    spriteBatch.Draw(_tileset, pos, source,Color.White);
                }
            }
        }

        _player.Draw(spriteBatch);
        spriteBatch.End();
    }
}