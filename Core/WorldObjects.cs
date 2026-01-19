using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class WorldObjects : Scene
{
    private Game _game;
    public WorldObjects(Game game) : base(game)
    {
        _game = game;
    }

    public override void LoadContent()
    {
        Texture2D note = _game.Content.Load<Texture2D>("note");
        TextureManager.NoteTexture = note;
        base.LoadContent();
    }
}