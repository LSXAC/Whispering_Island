using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class IslandBuildSave : Resource
{
    [Export]
    public Array<BeltSave> belt_saves = new Array<BeltSave>();

    [Export]
    public Array<BeltTransmitterSave> belt_transmitter_saves = new Array<BeltTransmitterSave>();

    [Export]
    public Array<MachineSave> machine_saves = new Array<MachineSave>();

    [Export]
    public Array<PlaceableSave> placeable_saves = new Array<PlaceableSave>();

    [Export]
    public Array<ResourceObjectSave> resource_obj_saves = new Array<ResourceObjectSave>();

    [Export]
    public int matrix_island_id = -1;

    public IslandBuildSave() { }
}
