using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;
public class TestScene : Scene
{
    private SpriteFont _font;
    private Map _map;
    private Player _player;
    private Camera _camera;

    public TestScene(Game game) : base(game) { }

    public override void LoadContent()
    {
        _font = Game.Content.Load<SpriteFont>("Munro");
        _player = new Player(Game.GraphicsDevice, new Vector2(10, 10));
        _map = new Map(Game);
        _map.LoadMap("Content/test-map.tmx", "tileset.png", _player);
      //  _map.SpawnEnemies(Game.GraphicsDevice);
        _camera = new Camera(Game.GraphicsDevice.Viewport);
    }

    public override void Update(GameTime gameTime)
    {
        _player.Update(gameTime);
        _map.Update(gameTime);         
        _camera.SetTarget(_player.Position + new Vector2(16, 16));
        _camera.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
        _map.Draw(spriteBatch);
        _player.Draw(spriteBatch);
        spriteBatch.End();
    }
}