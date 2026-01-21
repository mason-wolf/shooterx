using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Represents an object that can be interacted with.
/// </summary>
public abstract class InteractableObject : Scene
{

    public string Name { get; set; }
    public Texture2D Texture { get; set; }
    public Texture2D Position { get; set; }
    public Rectangle Rectangle { get; set; }
    public bool Destroyed { get; set; } = false;
    public abstract void Interact();

    private string _originalText;
    private string _currentText;

    public void SetInteractionText(string text)
    {
        if (_originalText == null)
            _originalText = text;
        _currentText = text;
    }

    public string GetInteractionText()
    {
        return _currentText;
    }

    public void ResetState()
    {
        _currentText = _originalText;
    }

    protected InteractableObject(Game game, Player player) : base(game)
    {
    }
}