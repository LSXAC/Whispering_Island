using System;
using Godot;

public partial class InventoryTab : ColorRect
{
    [Export]
    public Inventory inventory_ui,
        seed_ui;
    public static InventoryTab instance = null;

    public static SlotItemUI clicked_slot_item_ui = null;

    public override void _Ready()
    {
        instance = this;
    }

    public void OnVisiblitytityChanged()
    {
        inventory_ui.OnVisiblityChange();
        seed_ui.OnVisiblityChange();
        inventory_ui.Visible = true;
        seed_ui.Visible = false;
    }

    public void OnInventoryButton()
    {
        inventory_ui.Visible = true;
        seed_ui.Visible = false;
    }

    public void OnSeedButton()
    {
        inventory_ui.Visible = false;
        seed_ui.Visible = true;
    }
}
