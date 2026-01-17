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

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        float scale = 3f;
        Vector2 textSize = _font.MeasureString("PRESS START") * scale;
        Vector2 center = new Vector2(400, 100) - textSize * 0.5f; 
        
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        if ((int)(gameTime.TotalGameTime.TotalSeconds * 2) % 2 == 0)
        spriteBatch.DrawString(_font, "PRESS START", center, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        spriteBatch.End();
    }
}