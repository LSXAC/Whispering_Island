using System;
using System.Diagnostics;
using System.Net;
using Godot;

public partial class Minecart : MoveableBase
{
    [Export]
    public ChestBase chestBase;

    [Export]
    public Texture2D minecart_top,
        minecart_side;
    public bool is_running = false;

    //Receives Vector from Rail
    public Vector2 moving_vector = Vector2.Zero;

    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        MinecartTab.current_minecart = this;
        ChestInventory.INSTANCE = ((MinecartTab)GameMenu.INSTANCE.minecart_tab).chest_inventory;
        GameMenu.INSTANCE.OnOpenMinecartTab();
        ChestInventory.current_chest = chestBase;
        ChestInventory.INSTANCE.OpenChest();
    }

    public override void _Process(double delta)
    {
        if (is_running)
        {
            Debug.Print("Velo: " + moving_vector.ToString());
            if (moving_vector.X == 0 && moving_vector.Y == 0)
                return;
            if (moving_vector.Y == 0)
                GetSprite().Texture = minecart_side;
            if (moving_vector.X == 0)
                GetSprite().Texture = minecart_top;
        }
    }
}
