using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SmeltableAttribute : ItemAttributeBase
{
    [Export]
    public Item smelted_to_item;

    [Export]
    public int amount_to_smelt = 1;

    [Export]
    public Array<UnlockRequirement> unlock_requirements;
}
