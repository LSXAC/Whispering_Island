using System;
using Godot;

[GlobalClass]
public partial class ItemType : Resource
{
    [Export]
    public ItemInfo.Type type;
}
