using System;
using Godot;

[GlobalClass]
public partial class Item : Resource
{
    public Item() { }

    public Item(ItemInfo ii, int amount)
    {
        this.item_info = ii;
        this.amount = amount;
    }

    [Export]
    public ItemInfo item_info;

    [Export]
    public int amount = 0;
}
