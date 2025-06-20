using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Machine_Recipe : Resource
{
    [Export]
    public ItemResource import_item_resource;

    [Export]
    public ItemResource export_item_resource;

    [Export]
    public int import_amount = 0;

    [Export]
    public int export_amount = 0;

    [Export]
    public Array<UnlockRequirement> unlockRequirement;
}
