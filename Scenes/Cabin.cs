using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Cabin : Scene
{
    Player _player;
    Map _map;
    Camera _camera;
    public Cabin(Game game, Player player, Camera camera) : base(game)
    {
        _player = player;
        _camera = camera;
    }

    public override void LoadContent()
    {
        _player = new Player(Game.GraphicsDevice, new Vector2(422, 600));
        _map = new Map(Game);
        _map.LoadMap("Content/cabin.tmx", "tileset.png", _player);
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
            .SetStartingPosition(new Vector2(100, 100))
            // Wouldn't normally provide another starting position here,
            // but since its the first scene loaded it needs it.
            .SetScene(new Suburb(Game, new Vector2(100, 100)))
            .Execute();
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
        _map.Draw(spriteBatch, gameTime);
        _player.Draw(spriteBatch);
        TransitionManager.Draw(spriteBatch, new Rectangle(0, 0, 1920, 1080));
        spriteBatch.End();
    }
}