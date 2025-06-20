using System;
using Godot;

public partial class Item : Resource
{
    public Item() { }

    public Item(ItemResource resource, int amount)
    {
        this.resource = resource;
        this.amount = amount;
    }

    [Export]
    public ItemResource resource;

    [Export]
    public int amount;
}
