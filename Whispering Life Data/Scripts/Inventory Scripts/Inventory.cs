using System.Diagnostics;
using System.Transactions;

public partial class Inventory : InventoryBase
{
    public static Inventory INSTANCE = null;
    public static InventoryItem clicked_item = null;

    public override void _Ready()
    {
        INSTANCE = this;
        base._Ready();
        slot_amount = 30;
        SetSlots();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (clicked_item == null)
            return;

        if (Game_Manager.gameover)
        {
            clicked_item.Free();
            clicked_item = null;
            return;
        }

        clicked_item.GlobalPosition = GetGlobalMousePosition();
    }
}
