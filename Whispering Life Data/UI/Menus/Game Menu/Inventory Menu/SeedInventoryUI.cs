using System;
using Godot;

public partial class SeedInventoryUI : Inventory
{
    public static SeedInventoryUI instance = null;

    public override void _Ready()
    {
        instance = this;
        slot_amount = 30;
        base._Ready();
        SetSlots();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (InventoryTab.clicked_slot_item_ui == null)
            return;

        if (GameManager.gameover)
        {
            InventoryTab.clicked_slot_item_ui.Free();
            InventoryTab.clicked_slot_item_ui = null;
            return;
        }

        InventoryTab.clicked_slot_item_ui.GlobalPosition = GetGlobalMousePosition();
    }
}
