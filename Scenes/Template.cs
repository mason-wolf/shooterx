using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Template: Scene
{
    Player _player;
    Map _map;
    Camera _camera;

    public Template(Game game, Player player, Camera camera) : base(game)
    {
        _player = player;
        _camera = camera;
    }

    public override void LoadContent()
    {
        _map = new Map(Game);
        _map.LoadMap("Content/<map_name>.tmx", "tileset.png", _player);
        _map.SpawnEnemies(Game.GraphicsDevice);
    }

    public override void Update(GameTime gameTime)
    {
        _player.Update(gameTime);
        _map.Update(gameTime);
        _camera.SetTarget(_player.Position + new Vector2(16, 16));
        _camera.Update(gameTime);

        if (TransitionManager.IsActive)
        {
            TransitionManager.Update(gameTime);
            return;
        }

    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
        _map.Draw(spriteBatch, gameTime);
        _player.Draw(spriteBatch);
        DebugOverlay.Draw(spriteBatch, _player.Position);
        TransitionManager.Draw(spriteBatch, new Rectangle(0, 0, 1920, 1080));
        spriteBatch.End();
    }
}