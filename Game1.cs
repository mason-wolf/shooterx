using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace shooterx;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        // _graphics.PreferredBackBufferWidth = 1920;
        // _graphics.PreferredBackBufferHeight = 1080;
        // _graphics.IsFullScreen = true;
        // _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        GameState.ChangeScene(new MenuScene(this));
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
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