using System;
using Godot;

public partial class ItemSave : Resource
{
    [Export]
    public int item_id = -1;

    [Export]
    public int amount = 0;

    [Export]
    public int current_durability = -1;

    [Export]
    public int state = 0;

    public ItemSave() { }

    public ItemSave(int item_id, int amount, int current_durability = -1, int state = 0)
    {
        this.item_id = item_id;
        this.amount = amount;
        this.state = state;
        if (current_durability != -1)
            this.current_durability = current_durability;
    }
}
