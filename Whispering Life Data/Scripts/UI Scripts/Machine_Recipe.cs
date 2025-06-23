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
    public int amount_in_import = 0;

    [Export]
    public int amount_in_export = 0;

    [Export]
    public Array<UnlockRequirement> unlockRequirement;
}
