using Microsoft.Xna.Framework;

/// <summary>
/// Represents an object that can be interacted with.
/// </summary>
public abstract class InteractableObject : Scene
{
    public Rectangle Rectangle { get; set; }
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

    protected InteractableObject(Game game) : base(game)
    {
    }
}