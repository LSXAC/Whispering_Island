using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Building_Placer : Node2D
{
    [Export]
    public Node2D parent_Node;

    [Export]
    public TileMap tilemap;
    public static Node2D current_building = null;
    private placeable_building placeable;
    private Belt belt;
    private int current_belt_rotation = 3;
    public static PackedScene building = null;

    public Array<BeltSave> belt_saves = new Array<BeltSave>();

    public Array<MachineSave> machine_saves = new Array<MachineSave>();

    public void InitBuilding(PackedScene scene)
    {
        if (scene == null)
        {
            GD.PrintErr("Building is Null, can not be initialised");
            Building_Menu.instance.Visible = false;
            Game_Manager.building_mode = Game_Manager.BuildingMode.None;
            return;
        }
        Game_Manager.building_mode = Game_Manager.BuildingMode.Placing;
        player_ui.INSTANCE.SetWindowFrame();
        current_building = (Node2D)scene.Instantiate();

        building = scene;
        parent_Node.AddChild(current_building);
        if (current_building is Belt)
        {
            belt = current_building as Belt;
            belt.Set_Rotation(current_belt_rotation);
            belt.GetNode<Area2D>("BeltArea").Monitorable = false;
            belt.collision_shape.Disabled = true;
            return;
        }
        if (current_building is placeable_building)
        {
            placeable = current_building as placeable_building;
            placeable.sprite.SelfModulate = new Color(1f, 1f, 1f, 0.5f);
            placeable.collision_shape.Disabled = true;
            if (placeable.building_content != null)
                placeable.building_content.Visible = false;
        }
    }

    public void LoadBelts()
    {
        foreach (BeltSave belt_save in belt_saves)
        {
            Debug.Print("Belt Load" + " | " + belt_save.position);
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
            MachineBase temp = null;
            if (machine_save.type == MachineBase.MachineType.WOODFARM)
                temp = Building_Menu.instance.tree_growther.Instantiate() as ProductionMachine;

            if (machine_save.type == MachineBase.MachineType.QUARRY)
                temp = Building_Menu.instance.quarry.Instantiate() as ProductionMachine;

            if (machine_save.type == MachineBase.MachineType.FURNACE)
                temp = Building_Menu.instance.furnace.Instantiate() as ProcessBuilding;

            if (temp == null)
                return;

            parent_Node.AddChild(temp);
            temp.GlobalPosition = machine_save.position;
            temp.export_count = machine_save.export_count;
            temp.import_count = machine_save.import_count;
        }
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
                Debug.Print("Belt" + " | " + node.GlobalPosition + " | " + node.Position);
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
                Debug.Print("Machine" + " | " + node.GlobalPosition + " | " + node.Position);
                machine_saves.Add(
                    new MachineSave(
                        ((MachineBase)node).type,
                        ((MachineBase)node).Position,
                        ((MachineBase)node).export_count,
                        ((MachineBase)node).import_count
                    )
                );
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

        if (belt != null)
        {
            if (Input.IsActionJustPressed("Rotate_Right"))
            {
                current_belt_rotation++;
                if (current_belt_rotation == 4)
                    current_belt_rotation = 0;
                belt.Set_Rotation(current_belt_rotation);
            }
            if (Input.IsActionJustPressed("Rotate_Left"))
            {
                current_belt_rotation--;
                if (current_belt_rotation == -1)
                    current_belt_rotation = 3;
                belt.Set_Rotation(current_belt_rotation);
            }
        }

        if (Input.IsActionJustPressed("MouseLeft"))
        {
            Debug.Print("Mouse Pressed");
            if (placeable != null)
            {
                Debug.Print("placeable not null");
                if (placeable.building_collider_manager.AllCollidersOnBuildingLayer())
                {
                    BuildBuilding();
                    Debug.Print("Build!");
                }
            }
            if (belt != null)
            {
                if (belt.building_collider_manager.AllCollidersOnBuildingLayer())
                {
                    BuildBuilding();
                    Debug.Print("Build!");
                }
            }
        }
    }

    public void BuildBuilding()
    {
        Node2D temp = (Node2D)building.Instantiate();
        Vector2 pos = tilemap.LocalToMap(GetGlobalMousePosition());
        temp.Position = new Vector2(pos.X * 16, pos.Y * 16);
        parent_Node.AddChild(temp);

        if (temp is Belt)
        {
            ((Belt)temp).Set_Rotation(current_belt_rotation);
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
        belt = null;
        CloseMenuWithNoBuilding();
    }

    public void CloseMenuWithNoBuilding()
    {
        Game_Manager.building_mode = Game_Manager.BuildingMode.None;
        player_ui.INSTANCE.SetWindowFrame();
    }
}
