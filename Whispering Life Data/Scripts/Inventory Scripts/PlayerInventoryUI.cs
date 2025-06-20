using System.Diagnostics;
using System.Transactions;

public partial class PlayerInventoryUI : Inventory
{
    public static PlayerInventoryUI instance = null;
    public static SlotItem clicked_slot_item = null;

    public override void _Ready()
    {
        instance = this;
        slot_amount = 30;
        base._Ready();
        SetSlots();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (clicked_slot_item == null)
            return;

        if (GameManager.gameover)
        {
            clicked_slot_item.Free();
            clicked_slot_item = null;
            return;
        }

        clicked_slot_item.GlobalPosition = GetGlobalMousePosition();
    }
}
