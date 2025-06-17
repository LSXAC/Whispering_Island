using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ResearchLevel : Resource
{
    [Export]
    public Array<ItemInfo> unlocks_items;
}
