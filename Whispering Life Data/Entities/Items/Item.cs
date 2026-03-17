using System;
using Godot;

[GlobalClass]
public partial class Item : Resource
{
    public Item() { }

    public Item(ItemInfo info, int amount, STATE state = STATE.NORMAL)
    {
        this.info = info;
        this.amount = amount;
        this.state = state;
    }

    public Item Clone()
    {
        return new Item(this.info, this.amount, this.state);
    }

    [Export]
    public ItemInfo info;

    [Export]
    public int amount;

    [Export]
    public STATE state;

    public enum STATE
    {
        NORMAL,
        POISONED
    }
}
