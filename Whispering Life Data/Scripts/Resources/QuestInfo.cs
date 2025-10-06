using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class QuestInfo : Resource
{
    [Export]
    public string quest_name;

    [Export]
    public string quest_description;

    [Export]
    public Array<Item> required_items;

    [Export]
    public int quest_time;

    [Export]
    public int reward_money = 10;
}
