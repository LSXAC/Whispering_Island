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
        SetSlots();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (clicked_item == null)
            return;

        clicked_item.GlobalPosition = GetGlobalMousePosition();
    }
}
