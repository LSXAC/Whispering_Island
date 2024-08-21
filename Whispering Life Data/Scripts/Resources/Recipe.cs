using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Recipe : Resource
{
    [Export]
    public Array<Item> requiered_items;

    [Export]
    public Item output_item;
}
