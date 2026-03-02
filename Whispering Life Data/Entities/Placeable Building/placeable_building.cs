using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public abstract partial class placeable_building : Building_Node
{
    [Export]
    public Database.BUILDING_ID building_id;

    [Export]
    public bool disable_mouse_interaction = false;

    [Export]
    public Array<TILETYPE> tile_types;

    [Export]
    public bool can_be_build_on_air = false;

    private BuildingColliderManager building_collider_manager;
    public bool colliding_Wall = false;
    private MouseArea mouse_area;

    public enum TILETYPE
    {
        BUILDINGCOLLISION,
        FARMINGGROUND
    }

    public override void _Ready()
    {
        if (ignore_node_structure)
            return;
        base._Ready();
        mouse_area = GetNode<MouseArea>("MouseArea");
        building_collider_manager = GetNode<Node2D>("BuildingAreas") as BuildingColliderManager;

        if (disable_mouse_interaction)
            if (Logger.NodeIsNotNull(mouse_area))
                mouse_area.Monitorable = false;
    }

    public abstract Resource Save();

    public abstract void Load(Resource save);

    public bool CheckBuildingColliders()
    {
        if (Logger.NodeIsNotNull(building_collider_manager))
            if (building_collider_manager.AllCollidersOnBuildingLayer(this))
                return true;
        return false;
    }

    public void MakeSpriteTransparent()
    {
        if (Logger.NodeIsNotNull(sprite_anim_manager))
            sprite_anim_manager.MakeTransparent();
    }

    public void PrepareForBuild()
    {
        if (Logger.NodeIsNotNull(building_collider_manager))
            building_collider_manager.SetTileType(tile_types);
        ZIndex = 10;
        MakeSpriteTransparent();
        DisableCollision();
    }

    public override void OnMouseClick()
    {
        if (GameManager.building_mode == GameManager.BuildingMode.Removing)
        {
            sprite_anim_manager.shadowNode.RemoveShadow();
            QueueFree();
        }
    }

    public static bool CheckClickDependencies(Building_Node node)
    {
        if (GameMenu.IsWindowActiv() || GameManager.building_mode != GameManager.BuildingMode.None)
            return false;

        if (!node.mouse_inside)
            return false;

        if (GlobalFunctions.GetDistanceToPlayer(node.GlobalPosition) >= 50f)
            return false;

        return true;
    }

    public void SavePlaceable() { }

    public void LoadPlaceable(Resource save)
    {
        if (save is PlaceableSave placeable_save)
        {
            Position = placeable_save.pos;
            building_id = placeable_save.building_id;
        }
        else
            Logger.PrintWrongSaveType();
    }
}
