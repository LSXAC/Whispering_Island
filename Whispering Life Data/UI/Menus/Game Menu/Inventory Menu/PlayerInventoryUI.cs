using System.Diagnostics;
using System.Transactions;

public partial class PlayerInventoryUI : Inventory
{
    public static PlayerInventoryUI instance = null;
    public static SlotItemUI clicked_slot_item_ui = null;

    public override void _Ready()
    {
        instance = this;
        slot_amount = 30;
        base._Ready();
        SetSlots();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (clicked_slot_item_ui == null)
            return;

        if (GameManager.gameover)
        {
            clicked_slot_item_ui.Free();
            clicked_slot_item_ui = null;
            return;
        }

        clicked_slot_item_ui.GlobalPosition = GetGlobalMousePosition();
    }
}
