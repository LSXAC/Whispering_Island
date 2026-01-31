using System;
using Godot;
using Godot.Collections;

public partial class ResourceObjectManagerSave : Resource
{
    [Export]
    public Array<ResourceObjectSave> resource_object_saves = new Array<ResourceObjectSave>();

    [Export]
    public int matrix_island_id = -1;
}
