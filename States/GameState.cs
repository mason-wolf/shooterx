using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
    /// <summary>
    /// List of all enemies in the current game state.
    /// </summary>
    public static List<Enemy> Enemies = new List<Enemy>();

    /// <summary>
    /// List of custom map objects that are collidable.
    /// Track them for collision and object manipulation.
    /// </summary>
    public static List<Collidable> Collidables = new List<Collidable>();
    public static void ChangeScene(Scene newScene)
    {
        CurrentScene?.UnloadContent();
        CurrentScene = newScene;
        CurrentScene?.LoadContent();
    }
}