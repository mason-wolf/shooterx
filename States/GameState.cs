using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

public static class GameState
{
    /// <summary>
    /// Current active scene; includes UI, maps.
    /// </summary>
    public static Scene CurrentScene { get; private set; }
    /// <summary>
    /// Current active Tiled map.
    /// </summary>
    public static TmxMap CurrentMap { get; set; }
    /// <summary>
    /// Global font resource.
    /// </summary>
    public static SpriteFont Font { get; set; }
    /// <summary>
    /// Transition flag to indicate if a scene transition is in progress.
    /// </summary>
    public static bool Transitioning { get; set; } = false;
    public static void ChangeScene(Scene newScene)
    {
        CurrentScene?.UnloadContent();
        CurrentScene = newScene;
        CurrentScene?.LoadContent();
    }
}