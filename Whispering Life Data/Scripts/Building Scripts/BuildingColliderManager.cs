using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class BuildingColliderManager : Node2D
{
    [Export]
    private PackedScene building_collider = ResourceLoader.Load<PackedScene>(
        "res://Scenes/World Objects/Buildings/Building Components/Building_Collider_Component.tscn"
    );

    [Export]
    public PLACE_TYPE current_type = PLACE_TYPE.BUILDING;

    public enum PLACE_TYPE
    {
        BUILDING,
        MOVEABLE
    };

    [Export]
    private TileMapLayer collision_tilemap;

    public override void _Ready()
    {
        CreateBuildingCollider();
    }

    public void SetTileType(Array<placeable_building.TILETYPE> types)
    {
        foreach (Node2D node in GetChildren())
        {
            BuildingCollider bc = node as BuildingCollider;
            bc.types = types;
        }
    }

    public bool AllCollidersOnBuildingLayer(placeable_building building)
    {
        if (GetChildren().Count == 0)
            return false;

        foreach (Node2D node in GetChildren())
        {
            BuildingCollider bc = node as BuildingCollider;
            bc.type = current_type;
            bc.Calc(building.can_be_build_on_air);
            if (!bc.on_building_layer)
                return false;
        }
        return true;
    }

    private void CreateBuildingCollider()
    {
        if (Logger.NodeIsNotNull(collision_tilemap))
            foreach (Vector2I cell in collision_tilemap.GetUsedCells())
            {
                if (Logger.NodeIsNull(building_collider))
                    continue;

                Area2D area = (Area2D)building_collider.Instantiate();
                area.Position = new Vector2(cell.X * 16 + 8, cell.Y * 16 + 8);
                AddChild(area);
            }
    }
}
