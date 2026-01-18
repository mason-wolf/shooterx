using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class TransitionManager
{
    private static float timer;
    private static float duration;
    private static bool isFading;
    private static Texture2D blackTexture;
    private static Action midFadeAction;

    public static void Initialize(GraphicsDevice gd)
    {
        blackTexture = new Texture2D(gd, 1, 1);
        blackTexture.SetData(new[] { Color.Black });
    }

    public static void Start(Action changeSceneAction, float totalDuration = 1.5f)
    {
        duration = totalDuration;
        timer = totalDuration;
        isFading = true;
        midFadeAction = changeSceneAction;
        GameState.Transitioning = true;
    }

    public static void Update(GameTime gt)
    {
        if (!isFading) return;

        timer -= (float)gt.ElapsedGameTime.TotalSeconds;

        if (timer <= duration / 2f && midFadeAction != null)
        {
            midFadeAction.Invoke();
            midFadeAction = null;
        }

        if (timer <= 0)
        {
            isFading = false;
            GameState.Transitioning = false;
        }
    }

    public static void Draw(SpriteBatch sb, Rectangle screen)
    {
        if (!isFading) return;

        float half = duration / 2f;
        float alpha;

        if (timer > half)
            alpha = (duration - timer) / half;     
        else
            alpha = timer / half;                  

        alpha = MathHelper.Clamp(alpha, 0f, 1f);

        sb.Draw(blackTexture, screen, Color.Black * alpha);
    }

    public static bool IsActive => isFading;
}