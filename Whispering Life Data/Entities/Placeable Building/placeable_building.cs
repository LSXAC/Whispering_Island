using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public abstract partial class placeable_building : Building_Node
{
    [Export]
    public Database.BUILDING_ID building_id;

    [Export]
    public bool consums_magic_power = false;

    [Export]
    public float magic_power_consumption = 0f;

    public bool has_enough_magic_power = true;
    private PackedScene mp_missing_panel = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://cuyqy07pn2y0u")
    );
    public MpMissingPanel mp_missing_panel_instance;

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

    public abstract Resource Save();

    public abstract void Load(Resource save);

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

    public void InitMagicPower()
    {
        if (consums_magic_power)
        {
            mp_missing_panel_instance = mp_missing_panel.Instantiate<MpMissingPanel>();
            mp_missing_panel_instance.Visible = false;
            mp_missing_panel_instance.Position = new Vector2(-6f, -6f);
            AddChild(mp_missing_panel_instance);
        }
    }

    public override void OnMouseClick()
    {
        if (GameManager.building_mode == GameManager.BuildingMode.Removing)
        {
            sprite_anim_manager.shadowNode.RemoveShadow();
            QueueFree();
        }
    }

    public void ApplyMagicPowerConsumtionFromManager(MagicPowerListener listener)
    {
        if (consums_magic_power)
            listener.AddPowerConsumtion(magic_power_consumption);
    }

    public void UpdateMagicPowerBuilding(bool enough_power)
    {
        if (enough_power)
        {
            if (mp_missing_panel_instance != null)
                mp_missing_panel_instance.Visible = false;
            has_enough_magic_power = true;
        }
        else if (mp_missing_panel_instance != null)
        {
            mp_missing_panel_instance.Visible = true;
            has_enough_magic_power = false;
        }
    }

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
