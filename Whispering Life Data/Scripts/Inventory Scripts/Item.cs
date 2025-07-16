using System;
using Godot;

[GlobalClass]
public partial class Item : Resource
{
    public Item() { }

    public Item(ItemInfo info, int amount)
    {
        this.info = info;
        this.amount = amount;
    }

    public Item Clone()
    {
        return new Item(this.info, this.amount);
    }

    [Export]
    public ItemInfo info;

    [Export]
    public int amount;
}
