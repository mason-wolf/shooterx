using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class LockedGate : InteractableObject
{
    private Player _player;
    public LockedGate(Game game, Player player) : base(game, player)
    {
        _player = player;
        Name = "locked-gate";
    }

    public override void Interact()
    {
        if (_player.ObjectKeys > 0)
        {

            _player.ObjectKeys--;

            SetInteractionText("Unlocked!");

            // This will work for a single gate. Search by id for multiple gates.
            var gate = GameState.Collidables
            .FirstOrDefault(c => c.Name == "locked-gate");

            if (gate != null)
                gate.Destroyed = true;
            
            // Destroy prompt too.
            Destroyed = true;
        }
        else
        {
            SetInteractionText("You don't have a key.");
        }
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.Draw(spriteBatch, gameTime);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}