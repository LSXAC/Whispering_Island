using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CraftingRecipe : Resource
{
    [Export]
    public Array<Item> required_items;

    [Export]
    public CraftingMenu.CATEGORY category;

    [Export]
    public Item output_item;

    [Export]
    public Array<UnlockRequirement> unlock_requirements;
}
