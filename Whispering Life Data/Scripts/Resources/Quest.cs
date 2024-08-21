using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Quest : Resource
{
    [Export]
    public string quest_name;

    [Export]
    public string quest_description;

    [Export]
    public Array<Item> quest_items;

    [Export]
    public int quest_time;
}
