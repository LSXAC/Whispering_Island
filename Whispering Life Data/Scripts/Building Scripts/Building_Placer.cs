using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using Godot;
using Godot.Collections;

public partial class Building_Placer : Node2D
{
    [Export]
    public Islands_Manager islands_manager;

    public static Node2D current_building = null;
    public static PackedScene building = null;
    public static Recipe building_recipe = null;
    private placeable_building placeable;
    private int current_belt_rotation = 3;
    private Vector2 current_scale = new Vector2(1, 1);

    private bool can_create = false;
    private bool is_flipped = false;

    public void InitBuilding(BuildingType building_tye)
    {
        if (building_tye == null || building_tye.building_scene == null)
        {
            GD.PrintErr("Building is Null, can not be initialised");
            Building_Menu.instance.Visible = false;
            Game_Manager.building_mode = Game_Manager.BuildingMode.None;
            return;
        }

        is_flipped = false;
        can_create = true;
        current_scale = new Vector2(1, 1);
        Game_Manager.building_mode = Game_Manager.BuildingMode.Placing;
        player_ui.INSTANCE.SetWindowFrame();
        player_ui.INSTANCE.item_row_manager.CanCreate(building_tye.building_recipe.requiered_items);
        current_building = (Node2D)building_tye.building_scene.Instantiate();
        building_recipe = building_tye.building_recipe;

        building = building_tye.building_scene;
        islands_manager
            .GetNearestIsland(GetGlobalMousePosition())
            .building_manager.AddChild(current_building);
        placeable = (placeable_building)current_building;
        placeable.building_collider_manager.SetTileType(placeable.tile_types);

        if (((placeable_building)current_building).collision_shape != null)
            ((placeable_building)current_building).collision_shape.Disabled = true;
        else
            Debug.Print("CollisionShape fehlt!!");

        if (current_building is Belt)
        {
            placeable = current_building as Belt;
            ((Belt)placeable).Set_Rotation(current_belt_rotation);
            placeable.GetNode<Area2D>("BeltArea").Monitorable = false;
            placeable.GetSprite().SelfModulate = new Color(1f, 1f, 1f, 0.75f);
            if (current_building is BeltTunnel)
                placeable.GetNode<TunnelArea>("TunnelArea").Monitoring = false;
            return;
        }

        if (current_building is ResourceObject)
        {
            placeable = current_building as ResourceObject;
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
        if (Game_Manager.building_mode == Game_Manager.BuildingMode.Removing)
            if (Input.IsActionJustPressed("Close") || Input.IsActionJustPressed("Escape"))
                CloseMenuWithNoBuilding();

        if (current_building == null)
            return;

        Vector2 pos = islands_manager
            .GetNearestIsland(GetGlobalMousePosition())
            .building_area.LocalToMap(GetGlobalMousePosition());
        current_building.GlobalPosition = new Vector2((pos.X + 1) * 16, (pos.Y + 1) * 16);

        if (Input.IsActionJustPressed("Escape"))
            CloseMenuWithBuildingSelected();

        if (placeable is Belt)
        {
            if (Input.IsActionJustPressed("Rotate_Right"))
                RotateBeltRight();

            if (Input.IsActionJustPressed("Rotate_Left"))
                RotateBeltLeft();
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

    private void RotateBeltLeft()
    {
        current_belt_rotation--;
        if (current_belt_rotation == -1)
            current_belt_rotation = 3;
        ((Belt)placeable).Set_Rotation(current_belt_rotation);
    }

    private void RotateBeltRight()
    {
        current_belt_rotation++;
        if (current_belt_rotation == 4)
            current_belt_rotation = 0;
        ((Belt)placeable).Set_Rotation(current_belt_rotation);
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
        Island_Properties ip = islands_manager.GetNearestIsland(GetGlobalMousePosition());
        Vector2 pos = ip.building_area.LocalToMap(GetGlobalMousePosition() - ip.GlobalPosition); // Global Mouse Position needs to be subtracted to local Tilemap world point
        temp.GlobalPosition = new Vector2((pos.X + 1) * 16, (pos.Y + 1) * 16);
        temp.Scale = current_scale;
        ip.building_manager.AddChild(temp);
        // Remove Resources

        foreach (Item i in building_recipe.requiered_items)
            Inventory.INSTANCE.RemoveItem(
                i.item_info,
                i.amount,
                Inventory.INSTANCE.inventory_items
            );

        if (temp is ResourceObject)
        {
            ((ResourceObject)temp).SpawnPlant();
            if (!player_ui.INSTANCE.item_row_manager.CanCreate(building_recipe.requiered_items))
                can_create = false;
            return;
        }

        if (temp is Belt)
        {
            ((Belt)temp).Set_Rotation(current_belt_rotation);
            if (temp is BeltTunnel)
            {
                Debug.Print("BeltTunnel XX");
                Random rnd = new Random();
                temp.Name = "BeltTunnel + " + rnd.Next(0, 10000);
                ((BeltTunnel)temp).CheckIfTunnelInDir();
            }
            if (!player_ui.INSTANCE.item_row_manager.CanCreate(building_recipe.requiered_items))
                can_create = false;
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
                            case Belt.BeltDirection.Top:
                                giv.direction_not_giving = Belt.BeltDirection.Down;
                                break;
                            case Belt.BeltDirection.Down:
                                giv.direction_not_giving = Belt.BeltDirection.Top;
                                break;
                            case Belt.BeltDirection.Right:
                                giv.direction_not_giving = Belt.BeltDirection.Left;
                                break;
                            case Belt.BeltDirection.Left:
                                giv.direction_not_giving = Belt.BeltDirection.Right;
                                break;
                        }
                    }
                }
            }
        }
        CloseMenuWithBuildingSelected();
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
        Game_Manager.building_mode = Game_Manager.BuildingMode.None;
        player_ui.INSTANCE.SetWindowFrame();
        Building_Menu.instance.CloseWindow();
    }
}
