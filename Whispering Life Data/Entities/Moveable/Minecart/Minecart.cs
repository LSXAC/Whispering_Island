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

        ChestInventoryUI.instance = (
            (MinecartTab)GameMenu.instance.minecart_tab
        ).chest_inventory_ui;
        GameMenu.instance.OnOpenMinecartTab();
        ChestInventoryUI.current_chest = chestBase;
        ChestInventoryUI.instance.OpenChest();
    }

    //TODO: Animation by moving
    public override void _Process(double delta)
    {
        if (is_running)
        {
            if (moving_vector.X == 0 && moving_vector.Y == 0)
                return;
            //if (moving_vector.Y == 0)
            //    GetSprite().Texture = minecart_side;
            //if (moving_vector.X == 0)
            //    GetSprite().Texture = minecart_top;
        }
    }
}
