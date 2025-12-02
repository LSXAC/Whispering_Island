using System;
using Godot;

public partial class DurabilityComponent : Node2D
{
    [Export]
    public int durability;
    bool is_broken = false;

    public bool IsBroken()
    {
        return is_broken;
    }

    public void RemoveDurability(int amount)
    {
        if (is_broken)
            return;

        durability -= amount;
        if (durability <= 0)
        {
            durability = 0;
            is_broken = true;
        }
    }
}
