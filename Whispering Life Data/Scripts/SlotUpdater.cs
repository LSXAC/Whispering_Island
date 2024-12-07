using System;
using Godot;

public abstract partial class SlotUpdater : Control
{
    public abstract void UpdateSlot(int index, InventoryItem ii);

    public abstract void ClearSlot(int index);

    public abstract ItemInfo GetItemInfo(int index);
}
