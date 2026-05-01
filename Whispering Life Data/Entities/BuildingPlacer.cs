using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using Godot;
using Godot.Collections;

public partial class BuildingPlacer : Node2D
{
    [Export]
    public IslandManager island_manager;

    public static Node2D current_building = null;
    public static PackedScene selected_building = null;
    private placeable_building placeable;
    private Vector2 current_scale = new Vector2(1, 1);

    private bool can_create = false;
    private bool is_flipped = false;

    public static Node2D moveable_selected_parent = null;
    private Island current_island = null;

    private int amount_to_build = 0;

    [Signal]
    public delegate void BuildingPlacedEventHandler();

    public void InitBuildingFromBuildingMenu(
        Building_Menu_List_Object scene_info,
        int amount_to_build
    )
    {
        this.amount_to_build = amount_to_build;
        if (Logger.NodeIsNull(scene_info?.scene))
        {
            BuildMenu.instance.CloseWindow();
            GameManager.building_mode = GameManager.BuildingMode.None;
            return;
        }

        InitPlacingParameter(scene_info);

        AddChild(current_building);
        placeable = current_building as placeable_building;
        placeable.PrepareForBuild();

        if (current_building is TransportBase transport_base)
        {
            placeable = transport_base;
            transport_base.SetRotationAndDisableMonitoring(rotation: 3);
        }

        if (current_building is MineableObject mineable)
        {
            mineable.SetVariation(0, growth: true);
            placeable = mineable;
        }

        if (placeable is MachineBase machine_base)
            machine_base.DisableAreas();
    }

    private void InitPlacingParameter(Building_Menu_List_Object scene_info)
    {
        selected_building = scene_info.scene;
        current_building = (Node2D)scene_info.scene.Instantiate();
        is_flipped = false;
        can_create = true;
        current_scale = new Vector2(1, 1);
        GameManager.building_mode = GameManager.BuildingMode.Placing;
    }

    public override void _Process(double delta)
    {
        if (GameManager.building_mode == GameManager.BuildingMode.Removing)
        {
            if (Input.IsActionJustPressed("Close"))
                CloseMenuWithNoBuilding();

            if (Input.IsActionJustPressed("Escape"))
            {
                CloseMenuWithNoBuilding();
                PlayerUI.instance?.equipmentSelectBar?.ResetCurrentSlotToSelectState();
            }

            return;
        }

        if (current_building == null)
            return;

        current_island = island_manager.GetNearestIsland(GetGlobalMousePosition());
        current_building.GlobalPosition = GetBuildingPositionVec2(current_island);

        if (Input.IsActionJustPressed("Escape"))
        {
            CloseMenuWithBuildingSelected();
            PlayerUI.instance?.equipmentSelectBar?.ResetCurrentSlotToSelectState();
            return;
        }

        HandleRotationInput();

        if (Input.IsActionJustPressed("MouseLeft") && placeable != null)
            PlaceBuilding();
    }

    private void HandleRotationInput()
    {
        bool is_belt_or_rail =
            (placeable is Belt && placeable is not BeltCombiner && placeable is not BeltSplitter)
            || placeable is Rail;

        if (is_belt_or_rail)
        {
            if (Input.IsActionJustPressed("Rotate_Right"))
                ((TransportBase)placeable).RotateRight();

            if (Input.IsActionJustPressed("Rotate_Left"))
                ((TransportBase)placeable).RotateLeft();
        }
        else
        {
            if (
                Input.IsActionJustPressed("Rotate_Right")
                || Input.IsActionJustPressed("Rotate_Left")
            )
            {
                current_scale = new Vector2(placeable.Scale.X * -1, 1);
                placeable.Scale = current_scale;

                if (placeable is MachineBase)
                    is_flipped = !is_flipped;
            }
        }
    }

    private void PlaceBuilding()
    {
        if (placeable.CheckBuildingColliders())
            BuildBuilding();
    }

    public void BuildBuilding()
    {
        if (!can_create)
            return;

        placeable_building temp = (placeable_building)selected_building.Instantiate();
        // Gebäude als Child der Insel platzieren und lokale Position setzen
        if (temp is not Minecart)
            current_island.island_object_save_manager.AddChild(temp);
        // Lokale Tile-Position bestimmen
        Vector2 globalMouse = GetGlobalMousePosition();
        Vector2I localTile = current_island.building_area.LocalToMap(
            globalMouse - current_island.building_area.GlobalPosition
        );
        temp.Position = current_island.building_area.MapToLocal(localTile) + new Vector2(8, 8);
        temp.Scale = current_scale;

        SetBuildBuildingByBase(temp);
        EmitSignal(SignalName.BuildingPlaced);
        amount_to_build--;

        if (amount_to_build < 1)
        {
            can_create = false;
            CloseMenuWithBuildingSelected();
        }
    }

    public void SetBuildBuildingByBase(Node2D temp)
    {
        if (temp is Minecart)
        {
            if (moveable_selected_parent == null)
                return;
            SetBuildingWithMoveableBase(temp);
        }

        if (temp is MineableObject mineable_object)
        {
            int var = ((MineableObject)placeable).variant;
            mineable_object.SetVariation(var, growth: true);
            mineable_object.SpawnPlant();

            ((MineableObject)placeable).SetVariation(
                mineable_object.GetRandomVariation(),
                growth: true
            );
        }

        if (temp is TransportBase transport_base)
        {
            transport_base.Set_Rotation(((TransportBase)current_building).current_rotation);
            if (temp is BeltTunnel belt_tunnel)
                belt_tunnel.CheckIfTunnelInDir();
        }

        if (temp is MachineBase mb)
        {
            mb.EnableAreas();
            if (mb is OreMinerMachine oreMiner)
                oreMiner.InitOreMiner();
            if (mb is NerveTransducer transducer)
                NervTransducterManager.instance.AddNervTransducer(transducer);
        }
    }

    public void SetBuildingWithMoveableBase(Node2D node)
    {
        node.Position = Vector2.Zero;
        moveable_selected_parent.AddChild(node);
        moveable_selected_parent = null;
    }

    public Vector2 GetBuildingPositionVec2(Island island)
    {
        Vector2 globalMouse = GetGlobalMousePosition();
        Vector2I localTile = island.building_area.LocalToMap(
            globalMouse - island.building_area.GlobalPosition
        );
        Vector2 tileCenter =
            island.building_area.MapToLocal(localTile)
            + island.building_area.GlobalPosition
            + new Vector2(8, 8);
        return tileCenter;
    }

    public void CloseMenuWithBuildingSelected()
    {
        if (((Building_Node)current_building)?.sprite_anim_manager?.shadowNode != null)
            ((Building_Node)current_building).sprite_anim_manager.shadowNode.RemoveShadow();
        current_building.QueueFree();
        current_building = null;
        placeable = null;
        CloseMenuWithNoBuilding();
    }

    public void CloseMenuWithNoBuilding()
    {
        GameManager.building_mode = GameManager.BuildingMode.None;
        PlayerUI.instance.SetWindowFrame();
        BuildMenu.instance.CloseWindow();
    }
}
