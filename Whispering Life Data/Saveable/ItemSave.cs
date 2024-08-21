using System;
using Godot;

public partial class ItemSave : Resource
{
    [Export]
    public int item_id = -1;

    [Export]
    public int amount = 0;

    [Export]
    public int slot_id = -1;

    public ItemSave() { }

    public ItemSave(int item_id, int amount, int slot_id)
    {
        this.item_id = item_id;
        this.amount = amount;
        this.slot_id = slot_id;
    }
}
