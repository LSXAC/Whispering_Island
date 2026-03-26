using System;
using Godot;

public partial class CombinerTab : SlotUpdater
{
    [Export]
    public Slot import_1_slot;

    public static CombinerTab instance = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        instance = this;
    }

    public override void UpdateSlot(int index, SlotItemUI slot_item_ui)
    {
        throw new NotImplementedException();
    }

    public override void ClearSlot(int index)
    {
        throw new NotImplementedException();
    }

    public override ItemInfo GetItemInfo(int index)
    {
        throw new NotImplementedException();
    }
}
