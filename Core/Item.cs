using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Item
{
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Texture2D Texture { get; set; }
    public Rectangle Bounds => new Rectangle(
    (int)(Position.X - 8),  
    (int)(Position.Y - 8),
    16,
    16);
    public bool CanPickUp { get; set; } = false;
    public bool PickedUp { get; set; } = false;
    public int Value { get; set; } = 0;
}