using Microsoft.Xna.Framework;
using TiledSharp;

public class TeleportBuilder
{
    private GameTime gameTime;
    private string teleporterName;
    private Player player;
    private Camera camera;
    private Scene scene;
    private bool _teleported = false;
    private Vector2 startingPosition;
    public TeleportBuilder SetGameTime(GameTime gt) { gameTime = gt; return this; }
    public TeleportBuilder SetTeleporterName(string name) { teleporterName = name; return this; }
    public TeleportBuilder SetPlayer(Player p) { player = p; return this; }
    public TeleportBuilder SetCamera(Camera c) { camera = c; return this; }
    public TeleportBuilder SetScene(Scene s) { scene = s; return this; }
    public TeleportBuilder SetStartingPosition(Vector2 pos) { startingPosition = pos; return this; }

    public void Execute()
    {
        foreach (TmxObjectGroup group in GameState.CurrentMap.ObjectGroups)
        {
            foreach (TmxObject obj in group.Objects)
            {
                if (obj.Name == teleporterName)
                {
                    Rectangle objRect = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
                    if (player.Bounds.Intersects(objRect) && !_teleported)
                    {

                        TransitionManager.Start(() =>
                        {
                            player.Position = startingPosition;
                            camera.Position = player.Position;
                            GameState.ChangeScene(scene);
                        });
                        _teleported = true;
                    }
                }
            }
        }
    }
}