using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Machine_Recipe : Resource
{
    [Export]
    public Item import_item;

    [Export]
    public Item export_item;

    [Export]
    public Array<UnlockRequirement> unlockRequirement;
}
