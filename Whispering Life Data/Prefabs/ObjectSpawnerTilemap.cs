using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ObjectSpawnerTilemap : TileMap
{
    [Export]
    public PackedScene tree = ResourceLoader.Load<PackedScene>("res://Placeable/Tree.tscn");

    [Export]
    public PackedScene stone = ResourceLoader.Load<PackedScene>("res://Placeable/Stone.tscn");
    public ResourceObjectManagerSave roms = new ResourceObjectManagerSave();

    public Array<ResourceObject> resource_objects = new Array<ResourceObject>();

    public override void _Ready()
    {
        SetObjectsOnTilemap();
    }

    public void LoadResourceObjects(ResourceObjectManagerSave roms)
    {
        for (int i = 0; i < roms.resource_object_saves.Count; i++)
            resource_objects[i].ResourceObjectLoad(roms.resource_object_saves[i]);
    }

    public void SaveResourseObjects()
    {
        roms.resource_object_saves.Clear();

        for (int i = 0; i < resource_objects.Count; i++)
            roms.resource_object_saves.Add(resource_objects[i].SaveResourceObject());
    }

    private Vector2I woodVec = new Vector2I(0, 0);
    private Vector2I stoneVec = new Vector2I(1, 0);

    private void SetObjectsOnTilemap()
    {
        foreach (Vector2I cell2I in GetUsedCells(0))
        {
            ResourceObject bn = null;
            if (GetCellAtlasCoords(0, cell2I) == woodVec)
                bn = (ResourceObject)tree.Instantiate();
            if (GetCellAtlasCoords(0, cell2I) == stoneVec)
                bn = (ResourceObject)stone.Instantiate();

            if (bn == null)
                return;
            this.resource_objects.Add(bn);
            bn.Position = ToGlobal(MapToLocal(cell2I));
            AddChild(bn);
        }
    }
}
