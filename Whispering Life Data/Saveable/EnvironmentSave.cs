using System;
using Godot;
using Godot.Collections;

public partial class EnvironmentSave : Resource
{
    [Export]
    public Array<IslandSave> island_Saves = new Array<IslandSave>();

    [Export]
    public Array<ResourceObjectManagerSave> resource_object_manager_saves =
        new Array<ResourceObjectManagerSave>();
}
