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

        Texture2D money = _game.Content.Load<Texture2D>("money");
        TextureManager.MoneyTexture = money;

        Texture2D key = _game.Content.Load<Texture2D>("object-key");
        TextureManager.KeyTexture = key;

        base.LoadContent();
    }
}