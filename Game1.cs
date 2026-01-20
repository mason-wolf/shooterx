using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace shooterx;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private WorldObjects _worldObjects;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.HardwareModeSwitch = false; 
        _graphics.PreferredBackBufferWidth = 1920;    
        _graphics.PreferredBackBufferHeight = 1080;    
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        base.Initialize();
        GameState.ChangeScene(new MainMenu(this));
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _worldObjects = new WorldObjects(this);
        _worldObjects.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        GameState.CurrentScene?.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        GameState.CurrentScene?.Draw(_spriteBatch, gameTime);
        base.Draw(gameTime);
    }
}