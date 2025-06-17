using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Machine_Recipe : Resource
{
    [Export]
    public ItemInfo import_item_info;

    [Export]
    public ItemInfo export_item_info;

    [Export]
    public int import_amount = 0;

    [Export]
    public int export_amount = 0;

    [Export]
    public Array<UnlockRequirement> unlockRequirement;
}
