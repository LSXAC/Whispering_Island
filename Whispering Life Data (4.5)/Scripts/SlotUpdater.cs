using System;
using Godot;

public abstract partial class SlotUpdater : Control
{
    public abstract void UpdateSlot(int index, SlotItemUI slot_item_ui);

    public abstract void ClearSlot(int index);

    public abstract ItemInfo GetItemInfo(int index);
}
