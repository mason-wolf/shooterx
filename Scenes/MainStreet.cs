using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class MainStreet : Scene
{
    Player _player;
    Map _map;
    Camera _camera;
    HUD _hud;

    public MainStreet(Game game, Player player, Camera camera) : base(game)
    {
        _player = player;
        _camera = camera;
    }

    public override void LoadContent()
    {
        _map = new Map(Game);
        _map.LoadMap("Content/main_street.tmx", "tileset.png", _player);
        _map.SpawnEnemies(Game);
        _hud = new HUD(Game.GraphicsDevice);
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

            new TeleportBuilder()
        .SetGameTime(gameTime)
        .SetTeleporterName("suburb")
        .SetPlayer(_player)
        .SetCamera(_camera)
        .SetScene(new Suburb(Game, new Vector2(240, 12), _camera))
        .SetStartingPosition(new Vector2(50, 890))
        .Execute();
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
        _map.Draw(spriteBatch, gameTime);
        _player.Draw(spriteBatch);
        DebugOverlay.Draw(spriteBatch, _player.Position);
        TransitionManager.Draw(spriteBatch, new Rectangle(0, 0, 1920, 1080));
        spriteBatch.End();


        spriteBatch.Begin(transformMatrix: Matrix.Identity);
        _hud.Draw(spriteBatch, _player);
        spriteBatch.End();
    }
}