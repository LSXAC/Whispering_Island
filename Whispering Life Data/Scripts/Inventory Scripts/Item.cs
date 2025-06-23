using System;
using Godot;

public partial class Item : Resource
{
    public Item() { }

    public Item(ItemInfo info, int amount)
    {
        this.info = info;
        this.amount = amount;
    }

    [Export]
    public ItemInfo info;

    [Export]
    public int amount;
}
