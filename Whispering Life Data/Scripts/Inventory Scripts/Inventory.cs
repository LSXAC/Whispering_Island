using System.Diagnostics;
using System.Transactions;

public partial class Inventory : InventoryBase
{
    public static Inventory instance = null;
    public static InventoryItem clicked_item = null;

    public override void _Ready()
    {
        instance = this;
        slot_amount = 30;
        base._Ready();
        SetSlots();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (clicked_item == null)
            return;

        if (GameManager.gameover)
        {
            clicked_item.Free();
            clicked_item = null;
            return;
        }

        clicked_item.GlobalPosition = GetGlobalMousePosition();
    }
}
