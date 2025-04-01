using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Building_Collider_Manager : Node2D
{
    [Export]
    private PackedScene building_collider = ResourceLoader.Load<PackedScene>(
        "res://Placeable/BuildingCollider.tscn"
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

    public bool AllCollidersOnBuildingLayer()
    {
        if (GetChildren().Count == 0)
            return false;

        foreach (Node2D node in GetChildren())
        {
            BuildingCollider bc = node as BuildingCollider;
            bc.type = current_type;
            bc.Calc();
            if (!bc.on_building_layer)
                return false;
        }
        return true;
    }

    private void CreateBuildingCollider()
    {
        foreach (Vector2I cell in collision_tilemap.GetUsedCells())
        {
            Debug.Print("NEw Areas");
            Area2D area = (Area2D)building_collider.Instantiate();
            area.Position = new Vector2(cell.X * 16 + 8, cell.Y * 16 + 8);
            AddChild(area);
        }
    }
}
