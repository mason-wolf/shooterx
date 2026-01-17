using TiledSharp;

public static class GameState
{
    public static Scene CurrentScene { get; private set; }
    public static TmxMap CurrentMap { get; set; }
    public static void ChangeScene(Scene newScene)
    {
        CurrentScene?.UnloadContent();
        CurrentScene = newScene;
        CurrentScene?.LoadContent();
    }
}