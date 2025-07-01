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
    private int current_belt_rotation = 3;
    private Vector2 current_scale = new Vector2(1, 1);

    private Array<Item> required_items = new Array<Item>();

    private bool can_create = false;
    private bool is_flipped = false;

    public static Node2D moveable_selected_parent = null;

    public void InitBuildingFromBuildingMenu(Building_Menu_List_Object scene_info)
    {
        if (scene_info == null || scene_info.scene == null)
        {
            GD.PrintErr("Building is Null, can not be initialised");
            BuildMenu.instance.Visible = false;
            GameManager.building_mode = GameManager.BuildingMode.None;
            return;
        }

        // Grundzustand setzen
        selected_building = scene_info.scene;
        current_building = (Node2D)scene_info.scene.Instantiate();
        required_items = scene_info.required_items;
        is_flipped = false;
        can_create = true;
        current_scale = new Vector2(1, 1);
        GameManager.building_mode = GameManager.BuildingMode.Placing;

        // UI aktualisieren
        PlayerUI.instance.item_row_manager.CanCreate(required_items);
        PlayerUI.instance.SetWindowFrame();

        // Building ins Insel-Objekt einfügen
        Island nearestIsland = island_manager.GetNearestIsland(GetGlobalMousePosition());
        nearestIsland.island_object_save_manager.AddChild(current_building);

        // Placeable-Referenz setzen und Grundzustand
        placeable = current_building as placeable_building;
        if (placeable == null)
        {
            GD.PrintErr("Current building is not a placeable_building!");
            return;
        }
        placeable.building_collider_manager.SetTileType(placeable.tile_types);
        placeable.ZIndex = 10;
        placeable.GetSprite().SelfModulate = new Color(1f, 1f, 1f, 0.75f);

        // Spezialfälle behandeln
        if (current_building is Belt belt)
        {
            placeable = belt;
            belt.Set_Rotation(current_belt_rotation);
            belt.GetNode<Area2D>("PathConnectArea").Monitorable = false;
            if (belt is BeltTunnel tunnel)
                tunnel.GetNode<TunnelArea>("TunnelArea").Monitoring = false;
        }

        if (current_building is MineableObject mineable)
            placeable = mineable;

        if (placeable is MachineBase machine_base)
            machine_base.DisableTakers();

        // Collider deaktivieren
        if (placeable.collision_shape != null)
            placeable.collision_shape.Disabled = true;
        else
            Debug.Print("CollisionShape fehlt!!");
    }

    public override void _Process(double delta)
    {
        // Handle building mode removal
        if (GameManager.building_mode == GameManager.BuildingMode.Removing)
        {
            if (Input.IsActionJustPressed("Close") || Input.IsActionJustPressed("Escape"))
                CloseMenuWithNoBuilding();

            return;
        }

        // No building selected
        if (current_building == null)
            return;

        // Update building position
        Island island = island_manager.GetNearestIsland(GetGlobalMousePosition());
        current_building.GlobalPosition = GetBuildingPositionVec2(island);

        // Handle escape to cancel building
        if (Input.IsActionJustPressed("Escape"))
        {
            CloseMenuWithBuildingSelected();
            return;
        }

        // Handle rotation or flipping
        HandleRotationInput();

        // Place building on left mouse click
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
                RotateRight();

            if (Input.IsActionJustPressed("Rotate_Left"))
                RotateLeft();
        }
        else
        {
            if (
                Input.IsActionJustPressed("Rotate_Right")
                || Input.IsActionJustPressed("Rotate_Left")
            )
            {
                current_scale = new Vector2(placeable.Scale.X * -1, 1);
                if (placeable is MachineBase)
                {
                    is_flipped = !is_flipped;
                }
                placeable.Scale = current_scale;
            }
        }
    }

    private void RotateLeft()
    {
        current_belt_rotation--;
        if (current_belt_rotation == -1)
            current_belt_rotation = 3;
        ((TransportBase)placeable).Set_Rotation(current_belt_rotation);
    }

    private void RotateRight()
    {
        current_belt_rotation++;
        if (current_belt_rotation == 4)
            current_belt_rotation = 0;
        ((TransportBase)placeable).Set_Rotation(current_belt_rotation);
    }

    private void PlaceBuilding()
    {
        if (placeable == null)
            Debug.Print("Placeable NULL");
        if (placeable.building_collider_manager == null)
            Debug.Print("BCM NULL");
        if (placeable.building_collider_manager.AllCollidersOnBuildingLayer())
            BuildBuilding();
    }

    public void BuildBuilding()
    {
        if (!can_create)
            return;

        Node2D temp = (Node2D)selected_building.Instantiate();
        Island island = island_manager.GetNearestIsland(GetGlobalMousePosition());
        temp.GlobalPosition = GetBuildingPositionVec2(island);
        temp.Scale = current_scale;
        island.island_object_save_manager.AddChild(temp);

        BuildBuildingByBase(temp);
        RemoveBuildingResources();

        if (!PlayerUI.instance.item_row_manager.CanCreate(required_items))
        {
            can_create = false;
            CloseMenuWithBuildingSelected();
        }
    }

    public void BuildBuildingByBase(Node2D temp)
    {
        if (temp is Minecart)
        {
            if (moveable_selected_parent == null)
                return;
            SetBuildingWithMoveableBase(temp);
        }

        if (temp is MineableObject)
            ((MineableObject)temp).SpawnPlant();

        if (temp is TransportBase)
        {
            ((TransportBase)temp).Set_Rotation(current_belt_rotation);
            if (temp is BeltTunnel)
                ((BeltTunnel)temp).CheckIfTunnelInDir();
        }

        if (temp is MachineBase)
            SetBuildingWithMachineBase(temp);
    }

    public Vector2 GetBuildingPositionVec2(Island island)
    {
        Vector2 pos = island.building_area.LocalToMap(
            GetGlobalMousePosition() - island.GlobalPosition
        );
        return new Vector2((pos.X + 1) * 16, (pos.Y + 1) * 16);
    }

    public void SetBuildingWithMoveableBase(Node2D node)
    {
        node.Position = Vector2.Zero;
        moveable_selected_parent.AddChild(node);
        moveable_selected_parent = null;
    }

    public void SetBuildingWithMachineBase(Node2D node)
    {
        MachineBase pb = node as MachineBase;
        if (pb.givers != null)
        {
            foreach (Giver giv in pb.givers)
            {
                switch (giv.direction_not_giving)
                {
                    case TransportBase.Direction.Top:
                        giv.direction_not_giving = TransportBase.Direction.Down;
                        break;
                    case TransportBase.Direction.Down:
                        giv.direction_not_giving = TransportBase.Direction.Top;
                        break;
                    case TransportBase.Direction.Right:
                        giv.direction_not_giving = TransportBase.Direction.Left;
                        break;
                    case TransportBase.Direction.Left:
                        giv.direction_not_giving = TransportBase.Direction.Right;
                        break;
                }
            }
        }
    }

    private void RemoveBuildingResources()
    {
        foreach (Item item in required_items)
            PlayerInventoryUI.instance.RemoveItem(item, PlayerInventoryUI.instance.inventory_items);
    }

    public void CloseMenuWithBuildingSelected()
    {
        Node2D tempB = current_building;
        current_building = null;
        placeable = null;
        tempB.QueueFree();
        CloseMenuWithNoBuilding();
    }

    public void CloseMenuWithNoBuilding()
    {
        GameManager.building_mode = GameManager.BuildingMode.None;
        PlayerUI.instance.SetWindowFrame();
        BuildMenu.instance.CloseWindow();
    }
}
