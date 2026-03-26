using System;
using Godot;

public partial class ChestTab : ColorRect
{
    public static ChestTab instance;

    [Export]
    public ChestInventory chest_inventory;

    public override void _Ready()
    {
        instance = this;
        chest_inventory = GetNode<ChestInventory>("ChestInventory");
    }
}
