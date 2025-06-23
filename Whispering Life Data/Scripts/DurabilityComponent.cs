using Godot;
using System;

public partial class DurabilityComponent : Node2D
{
    [Export]
    public int max_durability;

    [Export]
    public int current_durability;
    bool is_broken = false;

    public bool IsBroken()
    {
        return is_broken;
    }

    public void RemoveDurability(int amount)
    {
        if (is_broken)
            return;

        current_durability -= amount;
        if (current_durability <= 0)
        {
            current_durability = 0;
            is_broken = true;
        }
    }
}
