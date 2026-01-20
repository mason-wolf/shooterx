using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

public class ItemPickupManager : Scene
{
    Player _player;
    List<Item> items = new List<Item>();
    TmxMap _map;
    public ItemPickupManager(Game game, TmxMap map, Player player) : base(game)
    {
        _player = player;
        _map = map;
        var objectLayer = _map.ObjectGroups.FirstOrDefault(l => l.Name == "items");

        if (objectLayer == null) return;

        foreach (var obj in objectLayer.Objects)
        {
            if (obj.Name == "first_aid_kit")
            {
                Item item = new Item();
                item.Name = "first_aid_kit";
                item.Position = new Vector2((float)obj.X, (float)obj.Y);
                item.Texture = TextureManager.FirstAidKitTexture;
                items.Add(item);
            }

            if (obj.Name == "money")
            {
                Item item = new Item();
                item.Name = "money";
                item.Position = new Vector2((float)obj.X, (float)obj.Y);
                item.Texture = TextureManager.MoneyTexture;

                var amountProp = obj.Properties.FirstOrDefault(p => p.Key == "amount");
                item.Value = int.Parse(amountProp.Value);

                items.Add(item);
            }

            if (obj.Name == "ammo")
            {
                Item item = new Item();
                item.Name = "ammo";
                item.Position = new Vector2((float)obj.X, (float)obj.Y);
                item.Texture = TextureManager.AmmoTexture;
                items.Add(item);
            }

            if (obj.Name == "key")
            {
                Item item = new Item();
                item.Name = "key";
                item.Position = new Vector2((float)obj.X, (float)obj.Y);
                item.Texture = TextureManager.KeyTexture;
                items.Add(item);
            }
        }

    }

    public override void Update(GameTime gameTime)
    {
        foreach (Item item in items)
        {
            if (_player.Bounds.Intersects(item.Bounds) && !item.PickedUp && item.Name == "first_aid_kit")
            {
                item.PickedUp = true;
                _player.FirstAidKits++;
            }

            if (_player.Bounds.Intersects(item.Bounds) && !item.PickedUp && item.Name == "money")
            {
                item.PickedUp = true;
                _player.Money += item.Value;
            }

            if (_player.Bounds.Intersects(item.Bounds) && !item.PickedUp && item.Name == "ammo")
            {
                item.PickedUp = true;
                _player.Ammo += 3;
            }

            if (_player.Bounds.Intersects(item.Bounds) && !item.PickedUp && item.Name == "key")
            {
                item.PickedUp = true;
                _player.ObjectKeys += 1;
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.Draw(spriteBatch, gameTime);
        foreach (var item in items)
        {
            if (!item.PickedUp)
            {
                spriteBatch.Draw(item.Texture, item.Position, Color.White);
            }
        }
    }
}
