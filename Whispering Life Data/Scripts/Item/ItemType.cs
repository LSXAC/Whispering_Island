using System;
using Godot;

[GlobalClass]
public partial class ItemType : Resource
{
    [Export]
    public ItemResource.TYPE type;
}
