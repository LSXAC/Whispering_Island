using System;
using Godot;

[GlobalClass]
public partial class SmeltableAttribute : ItemAttributeBase
{
    [Export]
    public Item smelted_to_item;

    [Export]
    public int amount_to_smelt = 1;
}
