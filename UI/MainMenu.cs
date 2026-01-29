using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class MainMenu : Scene
{
    SpriteFont _font;
    public MainMenu(Game game) : base(game) { }
    Vector2 PLAYER_START_POSITION = new Vector2(70, 700);
    public override void LoadContent()
    {
        _font = Game.Content.Load<SpriteFont>("Munro");
        GameState.Font = _font;
    }
    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            Camera camera = new Camera(Game.GraphicsDevice.Viewport);
            GameState.ChangeScene(new Suburb(Game, PLAYER_START_POSITION, camera));
        }
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        float scale = 3f;
        Vector2 textSize = _font.MeasureString("PRESS START") * scale;
        Vector2 center = new Vector2(
            Game.GraphicsDevice.Viewport.Width / 2f - textSize.X / 2f,
            Game.GraphicsDevice.Viewport.Height / 2f - textSize.Y / 2f);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        if ((int)(gameTime.TotalGameTime.TotalSeconds * 2) % 2 == 0)
            spriteBatch.DrawString(_font, "PRESS START", center, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        spriteBatch.End();
    }
}