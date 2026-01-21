using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class CabinBasement: Scene
{
    Player _player;
    Map _map;
    Camera _camera;
    HUD _hud;

    public CabinBasement(Game game, Player player, Camera camera) : base(game)
    {
        _player = player;
        _camera = camera;
    }

    public override void LoadContent()
    {
        _map = new Map(Game);
        _map.LoadMap("Content/cabin_basement.tmx", "tileset.png", _player);
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
            .SetTeleporterName("cabin")
            .SetPlayer(_player)
            .SetCamera(_camera)
            .SetStartingPosition(new Vector2(350, 570))
            .SetScene(new Cabin(Game, _player, _camera))
            .Execute();

    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
        _map.Draw(spriteBatch, gameTime);
        _player.Draw(spriteBatch);
        TransitionManager.Draw(spriteBatch, new Rectangle(0, 0, 1920, 1080));
        spriteBatch.End();


        spriteBatch.Begin(transformMatrix: Matrix.Identity);
        _hud.Draw(spriteBatch, _player);
        spriteBatch.End();
    }
}