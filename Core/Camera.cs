
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Camera
{
    public Vector2 Position;
    public Viewport viewport;
    private float lerpSpeed = 4f;
    private Vector2 target;

    public Camera(Viewport viewport)
    {
        this.viewport = viewport;
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Position = Vector2.Lerp(Position, target, MathHelper.Clamp(lerpSpeed * delta, 0f, 1f));
    }

    public void SetTarget(Vector2 newTarget)
    {
        target = newTarget;
    }
    public Matrix GetViewMatrix()
    {
        return Matrix.CreateTranslation(new Vector3(
            -Position.X + viewport.Width / 2f,
            -Position.Y + viewport.Height / 2f,
            0));
    }
}