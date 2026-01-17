public static class GameState
{
    public static Scene CurrentScene { get; private set; }

    public static void ChangeScene(Scene newScene)
    {
        CurrentScene?.UnloadContent();
        CurrentScene = newScene;
        CurrentScene?.LoadContent();
    }
}