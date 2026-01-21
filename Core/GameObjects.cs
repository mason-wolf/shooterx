using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GameObjects : Scene
{
    private Game _game;
    public GameObjects(Game game) : base(game)
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

        Texture2D lockedGate = _game.Content.Load<Texture2D>("locked-gate");
        TextureManager.LockedGate = lockedGate;
        base.LoadContent();
    }
}