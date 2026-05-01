using System;
using Godot;

public partial class ArmorInventoryUI : Inventory
{
    public static ArmorInventoryUI instance = null;

    public override void _Ready()
    {
        instance = this;
        slot_amount = 4;
        base._Ready();
        SetSlots();
    }
}
