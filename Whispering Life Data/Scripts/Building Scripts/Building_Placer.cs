using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using Godot;
using Godot.Collections;

public partial class Building_Placer : Node2D
{
    [Export]
    public IslandManager island_manager;

    public static Node2D current_building = null;
    public static PackedScene building = null;
    public static Recipe building_recipe = null;
    private placeable_building placeable;
    private int current_belt_rotation = 3;
    private Vector2 current_scale = new Vector2(1, 1);

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

        is_flipped = false;
        can_create = true;
        current_scale = new Vector2(1, 1);
        GameManager.building_mode = GameManager.BuildingMode.Placing;
        PlayerUI.instance.SetWindowFrame();
        PlayerUI.instance.item_row_manager.CanCreate(scene_info.recipe.required_items);
        current_building = (Node2D)scene_info.scene.Instantiate();
        building_recipe = scene_info.recipe;

        building = scene_info.scene;
        island_manager
            .GetNearestIsland(GetGlobalMousePosition())
            .island_object_save_manager.AddChild(current_building);
        placeable = (placeable_building)current_building;
        placeable.building_collider_manager.SetTileType(placeable.tile_types);

        if (((placeable_building)current_building).collision_shape != null)
            ((placeable_building)current_building).collision_shape.Disabled = true;
        else
            Debug.Print("CollisionShape fehlt!!");

        placeable.ZIndex = 10;

        if (current_building is Belt)
        {
            placeable = current_building as Belt;
            ((Belt)placeable).Set_Rotation(current_belt_rotation);
            placeable.GetNode<Area2D>("PathConnectArea").Monitorable = false;
            placeable.GetSprite().SelfModulate = new Color(1f, 1f, 1f, 0.75f);
            if (current_building is BeltTunnel)
                placeable.GetNode<TunnelArea>("TunnelArea").Monitoring = false;
            return;
        }

        if (current_building is MineableObject)
        {
            placeable = current_building as MineableObject;
            placeable.GetSprite().SelfModulate = new Color(1f, 1f, 1f, 0.75f);
            return;
        }

        if (current_building is placeable_building)
        {
            placeable = current_building as placeable_building;
            if (placeable is MachineBase)
            {
                foreach (Taker taker in ((MachineBase)placeable).takers)
                    taker.DisableMonitorable();
            }
            placeable.GetSprite().SelfModulate = new Color(1f, 1f, 1f, 0.75f);

            return;
        }
    }

    public override void _Process(double delta)
    {
        if (GameManager.building_mode == GameManager.BuildingMode.Removing)
            if (Input.IsActionJustPressed("Close") || Input.IsActionJustPressed("Escape"))
                CloseMenuWithNoBuilding();

        if (current_building == null)
            return;

        Vector2 pos = island_manager
            .GetNearestIsland(GetGlobalMousePosition())
            .building_area.LocalToMap(GetGlobalMousePosition());
        current_building.GlobalPosition = new Vector2((pos.X + 1) * 16, (pos.Y + 1) * 16);

        if (Input.IsActionJustPressed("Escape"))
            CloseMenuWithBuildingSelected();

        if (
            (
                placeable is Belt belt
                && placeable is not BeltCombiner
                && placeable is not BeltSplitter
            )
            || placeable is Rail
        )
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

        if (Input.IsActionJustPressed("MouseLeft"))
            if (placeable != null)
                PlaceBuilding();
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

        Node2D temp = (Node2D)building.Instantiate();
        if (temp is Minecart)
        {
            if (moveable_selected_parent == null)
                return;

            RemoveResources();
            temp.Position = Vector2.Zero;
            moveable_selected_parent.AddChild(temp);
            moveable_selected_parent = null;
            CloseMenuWithBuildingSelected();
            return;
        }
        Island island = island_manager.GetNearestIsland(GetGlobalMousePosition());
        Vector2 pos = island.building_area.LocalToMap(
            GetGlobalMousePosition() - island.GlobalPosition
        ); // Global Mouse Position needs to be subtracted to local Tilemap world point
        temp.GlobalPosition = new Vector2((pos.X + 1) * 16, (pos.Y + 1) * 16);
        temp.Scale = current_scale;
        island.island_object_save_manager.AddChild(temp);
        // Remove Resources
        RemoveResources();
        if (temp is MineableObject)
        {
            ((MineableObject)temp).SpawnPlant();
            if (!PlayerUI.instance.item_row_manager.CanCreate(building_recipe.required_items))
            {
                can_create = false;
                CloseMenuWithBuildingSelected();
            }
            return;
        }

        if (temp is TransportBase)
        {
            ((TransportBase)temp).Set_Rotation(current_belt_rotation);
            if (temp is BeltTunnel)
            {
                Debug.Print("BeltTunnel XX");
                Random rnd = new Random();
                temp.Name = "BeltTunnel + " + rnd.Next(0, 10000);
                ((BeltTunnel)temp).CheckIfTunnelInDir();
            }
            if (!PlayerUI.instance.item_row_manager.CanCreate(building_recipe.required_items))
            {
                can_create = false;
                CloseMenuWithBuildingSelected();
            }
            return;
        }

        if (temp is MachineBase)
        {
            if (is_flipped)
            {
                MachineBase pb = temp as MachineBase;
                if (pb.givers != null)
                {
                    foreach (Giver giv in pb.givers)
                    {
                        Debug.Print("GIver GIv");
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
        }
        CloseMenuWithBuildingSelected();
    }

    private void RemoveResources()
    {
        foreach (Item item in building_recipe.required_items)
            PlayerInventoryUI.instance.RemoveItem(item, PlayerInventoryUI.instance.inventory_items);
    }

    public void CloseMenuWithBuildingSelected()
    {
        Node2D tempB = current_building;
        current_building = null;
        building_recipe = null;
        tempB.QueueFree();
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
