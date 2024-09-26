using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ObjectSpawnerTilemap : TileMapLayer
{
    public PackedScene tree = ResourceLoader.Load<PackedScene>("res://Placeable/Tree.tscn");

    public PackedScene stone = ResourceLoader.Load<PackedScene>("res://Placeable/Stone.tscn");
    public PackedScene mystTree = ResourceLoader.Load<PackedScene>("res://Placeable/MystTree.tscn");
    public PackedScene mystFibre = ResourceLoader.Load<PackedScene>(
        "res://Placeable/MystFibre.tscn"
    );

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
    private Vector2I mystTreeVec = new Vector2I(2, 0);
    private Vector2I mystFibreVec = new Vector2I(3, 0);

    private void SetObjectsOnTilemap()
    {
        foreach (Vector2I cell2I in GetUsedCells())
        {
            ResourceObject bn = null;
            if (GetCellAtlasCoords(cell2I) == woodVec)
                bn = (ResourceObject)tree.Instantiate();
            if (GetCellAtlasCoords(cell2I) == stoneVec)
                bn = (ResourceObject)stone.Instantiate();

            if (GetCellAtlasCoords(cell2I) == mystTreeVec)
                bn = (ResourceObject)mystTree.Instantiate();
            if (GetCellAtlasCoords(cell2I) == mystFibreVec)
                bn = (ResourceObject)mystFibre.Instantiate();

            if (bn == null)
                return;

            this.resource_objects.Add(bn);
            bn.Position = ToGlobal(MapToLocal(cell2I));
            AddChild(bn);
        }
    }
}
