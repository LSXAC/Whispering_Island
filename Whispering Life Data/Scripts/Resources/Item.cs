using System;
using Godot;

[GlobalClass]
public partial class Item : Resource
{
    [Export]
    public ItemInfo item_info;

    [Export]
    public int amount = 0;
}
