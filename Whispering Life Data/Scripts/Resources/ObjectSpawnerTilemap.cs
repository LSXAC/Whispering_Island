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
    public PackedScene wheat = ResourceLoader.Load<PackedScene>("res://Placeable/Wheat.tscn");
    public PackedScene potato = ResourceLoader.Load<PackedScene>("res://Placeable/Potato.tscn");
    public PackedScene carrot = ResourceLoader.Load<PackedScene>("res://Placeable/Carrot.tscn");
    public PackedScene corn = ResourceLoader.Load<PackedScene>("res://Placeable/Corn.tscn");
    public PackedScene sand = ResourceLoader.Load<PackedScene>("res://Placeable/Sand.tscn");
    public PackedScene sand_stone = ResourceLoader.Load<PackedScene>(
        "res://Placeable/Sand_Stone.tscn"
    );
    public PackedScene iron_ore = ResourceLoader.Load<PackedScene>("res://Placeable/Iron_Ore.tscn");
    public PackedScene copper_ore = ResourceLoader.Load<PackedScene>(
        "res://Placeable/Copper_Ore.tscn"
    );

    public Array<ResourceObject> resource_objects = new Array<ResourceObject>();

    public override void _Ready()
    {
        SetObjectsOnTilemap();
    }

    private Vector2I woodVec = new Vector2I(0, 0);
    private Vector2I wheatVec = new Vector2I(0, 1);
    private Vector2I ironoreVec = new Vector2I(1, 1);
    private Vector2I potatoVec = new Vector2I(0, 2);
    private Vector2I carrotVec = new Vector2I(1, 2);
    private Vector2I cornVec = new Vector2I(2, 2);
    private Vector2I copperoreVec = new Vector2I(2, 1);
    private Vector2I sandVec = new Vector2I(0, 3);
    private Vector2I sandStoneVec = new Vector2I(1, 3);
    private Vector2I stoneVec = new Vector2I(1, 0);
    private Vector2I mystTreeVec = new Vector2I(2, 0);
    private Vector2I mystFibreVec = new Vector2I(3, 0);

    public void SetObjectsOnTilemap()
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

            if (GetCellAtlasCoords(cell2I) == wheatVec)
                bn = (ResourceObject)wheat.Instantiate();
            if (GetCellAtlasCoords(cell2I) == ironoreVec)
                bn = (ResourceObject)iron_ore.Instantiate();
            if (GetCellAtlasCoords(cell2I) == copperoreVec)
                bn = (ResourceObject)copper_ore.Instantiate();

            if (GetCellAtlasCoords(cell2I) == potatoVec)
                bn = (ResourceObject)potato.Instantiate();
            if (GetCellAtlasCoords(cell2I) == carrotVec)
                bn = (ResourceObject)carrot.Instantiate();
            if (GetCellAtlasCoords(cell2I) == cornVec)
                bn = (ResourceObject)corn.Instantiate();

            if (GetCellAtlasCoords(cell2I) == sandVec)
                bn = (ResourceObject)sand.Instantiate();
            if (GetCellAtlasCoords(cell2I) == sandStoneVec)
                bn = (ResourceObject)sand_stone.Instantiate();

            if (bn == null)
                return;

            this.resource_objects.Add(bn);
            bn.Position = ToGlobal(MapToLocal(cell2I));
            AddChild(bn);
        }
    }
}
