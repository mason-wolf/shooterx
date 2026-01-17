using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class MenuScene : Scene
{
    SpriteFont _font;
    public MenuScene(Game game) : base(game) { }

    public override void LoadContent()
    {
        // TODO: Need to load this only once.
        _font = Game.Content.Load<SpriteFont>("Munro");
    }
    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            GameState.ChangeScene(new TestScene(Game));
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.DrawString(_font, "Press ENTER", new Vector2(250, 200), Color.White);
        spriteBatch.End();
    }
}