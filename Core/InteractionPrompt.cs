using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class InteractionPrompt : InteractableObject
{
    public InteractionPrompt(Game game) : base(game)
    {
    }

    public override void Interact()
    {
        SetInteractionText("Okay");
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.Draw(spriteBatch, gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}