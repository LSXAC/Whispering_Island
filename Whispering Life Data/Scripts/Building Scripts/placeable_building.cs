using System;
using Godot;
using Godot.Collections;

public partial class placeable_building : Building_Node
{
    public bool colliding_Wall = false;

    [Export]
    public Database.BUILDING_ID building_id;

    [Export]
    public Array<TILETYPE> tile_types;

    [Export]
    public Building_Collider_Manager building_collider_manager;

    public enum TILETYPE
    {
        BUILDINGCOLLISION,
        FARMINGGROUND
    }

    public override void OnMouseClick()
    {
        if (Game_Manager.building_mode == Game_Manager.BuildingMode.Removing)
            QueueFree();
    }

    public static bool CheckClickDependencies(Building_Node node)
    {
        if (
            GameMenu.IsWindowActiv()
            || Game_Manager.building_mode != Game_Manager.BuildingMode.None
        )
            return false;

        if (!node.mouse_inside)
            return false;

        if (GlobalFunctions.GetDistanceToPlayer(node.GlobalPosition) >= 50f)
            return false;

        return true;
    }
}
