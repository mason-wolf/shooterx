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
        _font = Game.Content.Load<SpriteFont>("Munro");
        GameState.Font = _font;
    }
    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            GameState.ChangeScene(new Suburb(Game, new Vector2(50, 50)));
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