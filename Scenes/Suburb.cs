using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;
public class Suburb : Scene
{
    private SpriteFont _font;
    private Map _map;
    private Player _player;
    private Camera _camera;
    private Vector2 _startPosition;
    private TmxObject _currentNPC;
    private bool _isInteracting = false;
    public Suburb(Game game, Vector2 startPosition) : base(game)
    {
        _startPosition = startPosition;
    }

    public override void LoadContent()
    {
        _font = GameState.Font;
        _player = new Player(Game.GraphicsDevice, _startPosition);
        _map = new Map(Game);
        _map.LoadMap("Content/suburb.tmx", "tileset.png", _player);
        TransitionManager.Initialize(Game.GraphicsDevice);
        //_map.SpawnEnemies(Game.GraphicsDevice);
        _camera = new Camera(Game.GraphicsDevice.Viewport);
    }

    Rectangle npcRect;
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

        foreach (TmxObjectGroup group in GameState.CurrentMap.ObjectGroups)
        {
            foreach (TmxObject obj in group.Objects)
            {
                if (obj.Name == "npc")
                {
                    npcRect = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
                    if (_player.Bounds.Intersects(npcRect) && !_isInteracting)
                    {
                        Console.WriteLine("Hello there! Welcome to our suburb.");
                        _isInteracting = true;
                        _currentNPC = obj;
                    }
                }
            }
        }

        if (_currentNPC != null)
        {
            npcRect = new Rectangle((int)_currentNPC.X, (int)_currentNPC.Y, (int)_currentNPC.Width, (int)_currentNPC.Height);
            if (!_player.Bounds.Intersects(npcRect))
            {
                _currentNPC = null;
                _isInteracting = false;
            }
        }
        else
        {
            _currentNPC = null;
            _isInteracting = false;
        }

    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
        _map.Draw(spriteBatch, gameTime);
        _player.Draw(spriteBatch);
    
        if (_currentNPC != null)
        {
            Vector2 npcPos = new Vector2((float)(npcRect.X + npcRect.Width / 2), npcRect.Y - 30);
            Vector2 textSize = _font.MeasureString("(E) Talk") * 1.5f;
            npcPos -= textSize * 0.5f;
            spriteBatch.DrawString(_font, "(E) Talk", npcPos, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
        }
        TransitionManager.Draw(spriteBatch, new Rectangle(0, 0, 1920, 1080));
        spriteBatch.End();
    }
}