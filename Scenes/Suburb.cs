using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;
public class Suburb : Scene
{
    private Map _map;
    private Player _player;
    private Camera _camera;
    private Vector2 _startPosition;
    private Dialog _npcDialog;
    private HUD _hud;
    private WorldObjects _worldObjects;
    public Suburb(Game game, Vector2 startPosition, Camera camera) : base(game)
    {
        _startPosition = startPosition;
        _camera = camera;
    }

    public override void LoadContent()
    {

        _player = new Player(Game, _startPosition);
        _map = new Map(Game);
        _map.LoadMap("Content/suburb.tmx", "tileset.png", _player);
        TransitionManager.Initialize(Game.GraphicsDevice);
        _map.SpawnEnemies(Game);
        _npcDialog = new Dialog(Game, _player, "npc");
        _npcDialog.AddText("Hello, traveler! Welcome to the suburb.");
        _npcDialog.AddText("I hope you enjoy your stay here. Come back anytime!");

        _hud = new HUD(Game.GraphicsDevice);
        _worldObjects = new WorldObjects(Game);
        _worldObjects.LoadContent();
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
            .SetScene(new Cabin(Game, _player, _camera))
            .SetStartingPosition(new Vector2(422, 600))
            .Execute();

        _npcDialog.Update(gameTime);

    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
        _map.Draw(spriteBatch, gameTime);
        _player.Draw(spriteBatch);
      //  DebugOverlay.Draw(spriteBatch, _player.Position);
        TransitionManager.Draw(spriteBatch, new Rectangle(0, 0, 1920, 1080));
        _npcDialog.Draw(spriteBatch, gameTime);
        spriteBatch.End();

        spriteBatch.Begin(transformMatrix: Matrix.Identity);
        _hud.Draw(spriteBatch, _player);
        spriteBatch.End();
    }
}