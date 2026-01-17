using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public abstract class Scene
{
    protected Game Game;
    public Scene(Game game) => Game = game;

    public virtual void LoadContent() { }
    public virtual void UnloadContent() { }
    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }
}