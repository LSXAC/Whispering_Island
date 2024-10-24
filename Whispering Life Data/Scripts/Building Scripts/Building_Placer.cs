using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using Godot;
using Godot.Collections;

public partial class Building_Placer : Node2D
{
    [Export]
    public Node2D parent_Node;

    [Export]
    public TileMapLayer tilemap;
    public static Node2D current_building = null;
    public static PackedScene building = null;
    public Array<BeltSave> belt_saves = new Array<BeltSave>();
    public Array<MachineSave> machine_saves = new Array<MachineSave>();
    private placeable_building placeable;
    private int current_belt_rotation = 3;
    private Vector2 current_scale = new Vector2(1, 1);

    public void InitBuilding(PackedScene scene)
    {
        if (scene == null)
        {
            GD.PrintErr("Building is Null, can not be initialised");
            Building_Menu.instance.Visible = false;
            Game_Manager.building_mode = Game_Manager.BuildingMode.None;
            return;
        }
        current_scale = new Vector2(1, 1);
        Game_Manager.building_mode = Game_Manager.BuildingMode.Placing;
        player_ui.INSTANCE.SetWindowFrame();
        current_building = (Node2D)scene.Instantiate();

        building = scene;
        parent_Node.AddChild(current_building);
        if (current_building is Belt)
        {
            placeable = current_building as Belt;
            ((Belt)placeable).Set_Rotation(current_belt_rotation);
            placeable.GetNode<Area2D>("BeltArea").Monitorable = false;
            placeable.collision_shape.Disabled = true;
            if (current_building is BeltTunnel)
                placeable.GetNode<TunnelArea>("TunnelArea").Monitoring = false;
            return;
        }
        if (current_building is placeable_building)
        {
            placeable = current_building as placeable_building;
            placeable.GetSprite().SelfModulate = new Color(1f, 1f, 1f, 0.5f);
            placeable.collision_shape.Disabled = true;
            return;
        }
    }

    public void LoadBelts()
    {
        foreach (BeltSave belt_save in belt_saves)
        {
            Belt temp = Building_Menu.instance.belt.Instantiate() as Belt;
            parent_Node.AddChild(temp);
            temp.GlobalPosition = belt_save.position;
            temp.from_direction = belt_save.from_direction;
            temp.to_direction = belt_save.to_direction;
            temp.set_direction();
            temp.Set_Rotation(belt_save.current_rotation);

            if (belt_save.holded_item != null)
            {
                BeltItem item = (BeltItem)Building_Menu.instance.beltItem.Instantiate();
                Item ite = new Item(belt_save.holded_item, 1);
                item.InitBeltItem(ite);
                temp.item_holder.moving_item = belt_save.beltItem_moving;
                item.Position = belt_save.beltItem_position;
                temp.item_holder.AddChild(item);
            }
        }
    }

    public void LoadMachines()
    {
        foreach (MachineSave machine_save in machine_saves)
        {
            Debug.Print("Machine Load" + " | " + machine_save.position);
            MachineBase temp = SelectSavedMachine(machine_save);

            if (temp == null)
                return;

            parent_Node.AddChild(temp);
            temp.GlobalPosition = machine_save.position;
            temp.Scale = machine_save.scale;
            temp.machine_enabled = machine_save.machine_enabled;

            if (temp is ProcessBuilding)
            {
                ((ProcessBuilding)temp).export_count = machine_save.export_count;
                ((ProcessBuilding)temp).import_count = machine_save.import_count;
            }

            if (temp is Chest)
            {
                for (int i = 0; i < machine_save.chest_items.Length; i++)
                    ((Chest)temp).chest_items[i] = machine_save.chest_items[i];
            }
        }
    }

    private MachineBase SelectSavedMachine(MachineSave machine_save)
    {
        if (machine_save.type == MachineBase.MachineType.WOODFARM)
            return Building_Menu.instance.tree_growther.Instantiate() as ProductionMachine;

        if (machine_save.type == MachineBase.MachineType.QUARRY)
            return Building_Menu.instance.quarry.Instantiate() as ProductionMachine;

        if (machine_save.type == MachineBase.MachineType.FURNACE)
        {
            MachineBase temp = Building_Menu.instance.furnace.Instantiate() as ProcessBuilding;
            ((ProcessBuilding)temp).current_recipe = machine_save.current_recipe;

            if (machine_save.import_item_type != -1)
                ((ProcessBuilding)temp).import_item_info = Inventory.INSTANCE.item_Types[
                    machine_save.import_item_type
                ];

            if (machine_save.export_item_type != -1)
                ((ProcessBuilding)temp).export_item_info = Inventory.INSTANCE.item_Types[
                    machine_save.export_item_type
                ];
            return temp;
        }

        if (machine_save.type == MachineBase.MachineType.CHEST)
            return Building_Menu.instance.chest.Instantiate() as Chest;

        return null;
    }

    public void LoadPlacedObjects()
    {
        foreach (Node2D node in parent_Node.GetChildren())
            node.QueueFree();

        LoadBelts();
        LoadMachines();
    }

    public void SavePlacedObjects()
    {
        belt_saves.Clear();
        machine_saves.Clear();
        foreach (Node2D node in parent_Node.GetChildren())
        {
            if (node is Belt)
            {
                BeltSave belt_save = new BeltSave(
                    node.Position,
                    ((Belt)node).from_direction,
                    ((Belt)node).to_direction,
                    null,
                    ((Belt)node).current_rotation
                );

                if (((Belt)node).item_holder.hasBeltItem())
                {
                    belt_save.holded_item = ((Belt)node).item_holder.GetBeltItem().item.item_info;
                    belt_save.beltItem_moving = ((Belt)node).item_holder.moving_item;
                    belt_save.beltItem_position = ((Belt)node).item_holder.GetBeltItem().Position;
                }

                belt_saves.Add(belt_save);
            }

            if (node is MachineBase)
            {
                MachineSave ms = new MachineSave(
                    ((MachineBase)node).type,
                    ((MachineBase)node).Position,
                    ((MachineBase)node).Scale,
                    ((MachineBase)node).machine_enabled
                );

                if (node is ProcessBuilding)
                {
                    ms.current_recipe = ((ProcessBuilding)node).current_recipe;
                    ms.export_count = ((ProcessBuilding)node).export_count;
                    ms.import_count = ((ProcessBuilding)node).import_count;
                    if (((ProcessBuilding)node).import_item_info == null)
                        ms.import_item_type = -1;
                    else
                        ms.import_item_type = ((ProcessBuilding)node)
                            .import_item_info
                            .unique_item_id;
                    if (((ProcessBuilding)node).export_item_info == null)
                        ms.export_item_type = -1;
                    else
                        ms.export_item_type = ((ProcessBuilding)node)
                            .export_item_info
                            .unique_item_id;
                }

                if (node is Chest)
                    ms.chest_items = ((Chest)node).chest_items;

                machine_saves.Add(ms);
            }
        }
    }

    public override void _Process(double delta)
    {
        if (Game_Manager.building_mode == Game_Manager.BuildingMode.Removing)
            if (Input.IsActionJustPressed("Close"))
                CloseMenuWithNoBuilding();

        if (current_building == null)
            return;

        Vector2 pos = tilemap.LocalToMap(GetGlobalMousePosition());
        current_building.Position = new Vector2(pos.X * 16, pos.Y * 16);

        if (Input.IsActionJustPressed("Close"))
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
        Node2D temp = (Node2D)building.Instantiate();
        Vector2 pos = tilemap.LocalToMap(GetGlobalMousePosition());
        temp.Position = new Vector2(pos.X * 16, pos.Y * 16);
        temp.Scale = current_scale;
        parent_Node.AddChild(temp);

        if (temp is Belt)
        {
            ((Belt)temp).Set_Rotation(current_belt_rotation);
            if (temp is BeltTunnel)
            {
                Debug.Print("BeltTunnel XX");
                ((BeltTunnel)temp).CheckIfTunnelInDir();
            }
            return;
        }
        CloseMenuWithBuildingSelected();
    }

    public void CloseMenuWithBuildingSelected()
    {
        Node2D tempB = current_building;
        current_building = null;
        tempB.QueueFree();
        placeable = null;
        CloseMenuWithNoBuilding();
    }

    public void CloseMenuWithNoBuilding()
    {
        Game_Manager.building_mode = Game_Manager.BuildingMode.None;
        player_ui.INSTANCE.SetWindowFrame();
    }
}
