using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Cabin : Scene
{
    Player _player;
    Map _map;
    Camera _camera;
    Dialog _cabinComputerDialog;
    public Cabin(Game game, Player player, Camera camera) : base(game)
    {
        _player = player;
        _camera = camera;
    }

    public override void LoadContent()
    {
        _map = new Map(Game);
        _map.LoadMap("Content/cabin.tmx", "tileset.png", _player);
        _map.SpawnEnemies(Game.GraphicsDevice);
        _cabinComputerDialog = new Dialog(Game, _player, "cabin_computer");
        _cabinComputerDialog.SetInteractionText("(E) Access Computer");
        _cabinComputerDialog.AddText("You have an email from an unknown sender.");
        _cabinComputerDialog.AddText("'It's not real. None of it is.'");
        _cabinComputerDialog.AddText("--Check out ChartUpCorp's new Human-Charged Battery Cells!--");
        _cabinComputerDialog.AddText("The rest of the email is unreadable because of so many ads.");
    }

    public override void Update(GameTime gameTime)
    {
        _player.Update(gameTime);
        _map.Update(gameTime);
        _camera.SetTarget(_player.Position + new Vector2(16, 16));
        _camera.Update(gameTime);


        Vector2 teleportPos = new Vector2(194, 112);
        
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
            .SetStartingPosition(teleportPos)
            // Wouldn't normally provide another starting position here,
            // but since its the first scene loaded it needs it.
            .SetScene(new Suburb(Game, teleportPos, _camera))
            .Execute();
        
        new TeleportBuilder()
            .SetGameTime(gameTime)
            .SetTeleporterName("basement")
            .SetPlayer(_player)
            .SetCamera(_camera)
            .SetStartingPosition(new Vector2(190, 192))
            .SetScene(new CabinBasement(Game, _player, _camera))
            .Execute();
        _cabinComputerDialog.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
        _map.Draw(spriteBatch, gameTime);
        _player.Draw(spriteBatch);
        TransitionManager.Draw(spriteBatch, new Rectangle(0, 0, 1920, 1080));
        _cabinComputerDialog.Draw(spriteBatch, gameTime);
        spriteBatch.End();
    }
}