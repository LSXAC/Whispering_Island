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

        MinecartTab.instance.chest_inventory.OpenChest(chestBase.chest_items);
        GameMenu.instance.OnOpenMinecartTab();
    }

    //TODO: Animation by moving
    public override void _Process(double delta)
    {
        if (is_running)
        {
            if (moving_vector.X == 0 && moving_vector.Y == 0)
                return;
        }
    }
}
