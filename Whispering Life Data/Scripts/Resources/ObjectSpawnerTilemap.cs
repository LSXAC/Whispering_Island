using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ObjectSpawnerTilemap : TileMapLayer
{
    public PackedScene tree = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_oak_tree.tscn"
    );

    public PackedScene stone = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_stone.tscn"
    );
    public PackedScene mystTree = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_myst_oak_tree.tscn"
    );
    public PackedScene mystFibre = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_myst_fibre.tscn"
    );
    public PackedScene wheat = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_wheat.tscn"
    );
    public PackedScene potato = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_potato.tscn"
    );
    public PackedScene carrot = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_carrot.tscn"
    );
    public PackedScene corn = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_corn.tscn"
    );
    public PackedScene sand = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_sand.tscn"
    );
    public PackedScene sand_stone = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_sand_stone.tscn"
    );
    public PackedScene iron_ore = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_iron_ore.tscn"
    );
    public PackedScene copper_ore = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Mineable Objects/mineable_object_copper_ore.tscn"
    );

    public Array<MineableObject> resource_objects = new Array<MineableObject>();

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
            MineableObject bn = null;
            if (GetCellAtlasCoords(cell2I) == woodVec)
                bn = (MineableObject)tree.Instantiate();
            if (GetCellAtlasCoords(cell2I) == stoneVec)
                bn = (MineableObject)stone.Instantiate();

            if (GetCellAtlasCoords(cell2I) == mystTreeVec)
                bn = (MineableObject)mystTree.Instantiate();
            if (GetCellAtlasCoords(cell2I) == mystFibreVec)
                bn = (MineableObject)mystFibre.Instantiate();

            if (GetCellAtlasCoords(cell2I) == wheatVec)
                bn = (MineableObject)wheat.Instantiate();
            if (GetCellAtlasCoords(cell2I) == ironoreVec)
                bn = (MineableObject)iron_ore.Instantiate();
            if (GetCellAtlasCoords(cell2I) == copperoreVec)
                bn = (MineableObject)copper_ore.Instantiate();

            if (GetCellAtlasCoords(cell2I) == potatoVec)
                bn = (MineableObject)potato.Instantiate();
            if (GetCellAtlasCoords(cell2I) == carrotVec)
                bn = (MineableObject)carrot.Instantiate();
            if (GetCellAtlasCoords(cell2I) == cornVec)
                bn = (MineableObject)corn.Instantiate();

            if (GetCellAtlasCoords(cell2I) == sandVec)
                bn = (MineableObject)sand.Instantiate();
            if (GetCellAtlasCoords(cell2I) == sandStoneVec)
                bn = (MineableObject)sand_stone.Instantiate();

            if (bn == null)
                return;

            this.resource_objects.Add(bn);
            bn.Position = ToGlobal(MapToLocal(cell2I));
            AddChild(bn);
        }
    }
}
