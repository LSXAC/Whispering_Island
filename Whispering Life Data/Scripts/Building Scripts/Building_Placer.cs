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
    public static Recipe building_recipe = null;
    public Array<BeltSave> belt_saves = new Array<BeltSave>();
    public Array<BeltTransmitterSave> belt_transmitter_saves = new Array<BeltTransmitterSave>();
    public Array<MachineSave> machine_saves = new Array<MachineSave>();
    public Array<PlaceableSave> placeable_saves = new Array<PlaceableSave>();
    private placeable_building placeable;
    private int current_belt_rotation = 3;
    private Vector2 current_scale = new Vector2(1, 1);

    private PackedScene belt_item = ResourceLoader.Load<PackedScene>("res://belt_item.tscn");
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
        parent_Node.AddChild(current_building);
        if (current_building is Belt)
        {
            placeable = current_building as Belt;
            ((Belt)placeable).Set_Rotation(current_belt_rotation);
            placeable.GetNode<Area2D>("BeltArea").Monitorable = false;
            placeable.collision_shape.Disabled = true;
            placeable.GetSprite().SelfModulate = new Color(1f, 1f, 1f, 0.75f);
            if (current_building is BeltTunnel)
                placeable.GetNode<TunnelArea>("TunnelArea").Monitoring = false;
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
            placeable.collision_shape.Disabled = true;
            return;
        }
    }

    public void LoadBelts()
    {
        foreach (BeltSave belt_save in belt_saves)
        {
            Belt temp =
                Database.GetBuildingType(Database.BUILDING_ID.BELT).building_scene.Instantiate()
                as Belt;

            InitBelt(temp, belt_save);
        }
    }

    private void InitBelt(Belt belt, BeltSave beltsave)
    {
        if (belt == null || beltsave == null)
        {
            if (beltsave == null)
                Debug.Print("BeltSave Null");
            if (belt == null)
                Debug.Print("Belt Null");
            return;
        }
        parent_Node.AddChild(belt);
        belt.GlobalPosition = beltsave.position;
        belt.from_direction = beltsave.from_direction;
        belt.to_direction = beltsave.to_direction;
        belt.set_direction();
        belt.Set_Rotation(beltsave.current_rotation);

        if (beltsave.holded_item != null)
        {
            BeltItem item = (BeltItem)belt_item.Instantiate() as BeltItem;

            Item ite = new Item(beltsave.holded_item, 1);
            item.InitBeltItem(ite);
            belt.item_holder.moving_item = beltsave.beltItem_moving;
            item.Position = beltsave.beltItem_position;
            belt.item_holder.AddChild(item);
        }
    }

    public void LoadBeltTransmitter()
    {
        foreach (BeltTransmitterSave bts in belt_transmitter_saves)
        {
            if (bts.is_connected)
            {
                BeltTunnel temp =
                    Database
                        .GetBuildingType(Database.BUILDING_ID.BELTTUNNEL)
                        .building_scene.Instantiate() as BeltTunnel;

                InitBelt(temp, bts.beltsave1);

                BeltTunnel temp2 =
                    Database
                        .GetBuildingType(Database.BUILDING_ID.BELTTUNNEL)
                        .building_scene.Instantiate() as BeltTunnel;

                InitBelt(temp2, bts.beltsave2);

                temp.is_tunnel_connected = true;
                temp2.is_tunnel_connected = true;
                temp.connected_itemholder = temp2.item_holder;
                temp2.connected_itemholder = temp.item_holder;
            }
            else
            {
                BeltTunnel temp =
                    Database
                        .GetBuildingType(Database.BUILDING_ID.BELTTUNNEL)
                        .building_scene.Instantiate() as BeltTunnel;

                InitBelt(temp, bts.beltsave1);
            }
        }
    }

    public void LoadMachines()
    {
        foreach (MachineSave machine_save in machine_saves)
        {
            Debug.Print("Machine Load" + " | " + machine_save.pos);
            MachineBase temp = SelectSavedMachine(machine_save);

            if (temp == null)
                return;

            parent_Node.AddChild(temp);
            temp.GlobalPosition = machine_save.pos;
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

    public void LoadPlacableBuildings()
    {
        foreach (PlaceableSave ps in placeable_saves)
        {
            placeable_building temp = null;
            if (ps.building_id == Database.BUILDING_ID.RESEARCH_TABLE)
                temp =
                    Database
                        .GetBuildingType(Database.BUILDING_ID.RESEARCH_TABLE)
                        .building_scene.Instantiate() as ResearchTable;

            if (temp == null)
                return;

            parent_Node.AddChild(temp);
            temp.GlobalPosition = ps.pos;
        }
    }

    private MachineBase SelectSavedMachine(MachineSave machine_save)
    {
        if (machine_save.building_id == Database.BUILDING_ID.TREE_GROWTHER)
            return Database
                    .GetBuildingType(Database.BUILDING_ID.TREE_GROWTHER)
                    .building_scene.Instantiate() as ProductionMachine;

        if (machine_save.building_id == Database.BUILDING_ID.QUARRY)
            return Database
                    .GetBuildingType(Database.BUILDING_ID.QUARRY)
                    .building_scene.Instantiate() as ProductionMachine;

        if (machine_save.building_id == Database.BUILDING_ID.FURNACE)
        {
            MachineBase temp =
                Database.GetBuildingType(Database.BUILDING_ID.FURNACE).building_scene.Instantiate()
                as ProcessBuilding;
            ((ProcessBuilding)temp).current_recipe = machine_save.current_recipe;

            if (machine_save.import_item_type != -1)
                ((ProcessBuilding)temp).import_item_info = Inventory.INSTANCE.item_Types[
                    (Inventory.ITEM_ID)machine_save.import_item_type
                ];

            if (machine_save.export_item_type != -1)
                ((ProcessBuilding)temp).export_item_info = Inventory.INSTANCE.item_Types[
                    (Inventory.ITEM_ID)machine_save.export_item_type
                ];
            return temp;
        }

        if (machine_save.building_id == Database.BUILDING_ID.CHEST)
            return Database.GetBuildingType(Database.BUILDING_ID.CHEST).building_scene.Instantiate()
                as Chest;

        return null;
    }

    public void LoadPlacedObjects()
    {
        foreach (Node2D node in parent_Node.GetChildren())
            node.QueueFree();

        LoadBelts();
        LoadMachines();
        LoadBeltTransmitter();
        LoadPlacableBuildings();
    }

    public void SavePlacedObjects()
    {
        belt_saves.Clear();
        machine_saves.Clear();
        belt_transmitter_saves.Clear();
        placeable_saves.Clear();

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

                if (node is BeltTunnel)
                {
                    bool placed = false;
                    if (((BeltTunnel)node).is_tunnel_connected)
                        foreach (BeltTransmitterSave bts in belt_transmitter_saves)
                        {
                            if (bts.is_connected)
                                if (
                                    bts.beltsave1.position
                                    == ((BeltTunnel)node)
                                        .connected_itemholder.GetParent<BeltTunnel>()
                                        .Position
                                )
                                {
                                    bts.beltsave2 = belt_save;
                                    placed = true;
                                    break;
                                }
                        }
                    if (placed)
                        continue;

                    belt_transmitter_saves.Add(
                        new BeltTransmitterSave(
                            belt_save,
                            ((BeltTunnel)node).is_tunnel_connected,
                            null
                        )
                    );
                }
                else
                    belt_saves.Add(belt_save);
                continue;
            }

            if (node is MachineBase)
            {
                MachineSave ms = new MachineSave(
                    ((placeable_building)node).building_id,
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
                        ms.import_item_type = (int)
                            ((ProcessBuilding)node).import_item_info.unique_id;
                    if (((ProcessBuilding)node).export_item_info == null)
                        ms.export_item_type = -1;
                    else
                        ms.export_item_type = (int)
                            ((ProcessBuilding)node).export_item_info.unique_id;
                }

                if (node is Chest)
                    ms.chest_items = ((Chest)node).chest_items;

                machine_saves.Add(ms);
                continue;
            }

            if (node is placeable_building)
            {
                PlaceableSave ps = new PlaceableSave(
                    ((placeable_building)node).building_id,
                    node.GlobalPosition
                );
                placeable_saves.Add(ps);
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
        Vector2 pos = tilemap.LocalToMap(GetGlobalMousePosition());
        temp.Position = new Vector2(pos.X * 16, pos.Y * 16);
        temp.Scale = current_scale;
        parent_Node.AddChild(temp);
        // Remove Resources

        foreach (Item i in building_recipe.requiered_items)
            Inventory.INSTANCE.AddItem(i.item_info, -i.amount, Inventory.INSTANCE.inventory_items);

        if (temp is Belt)
        {
            ((Belt)temp).Set_Rotation(current_belt_rotation);
            if (temp is BeltTunnel)
            {
                Debug.Print("BeltTunnel XX");
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
    }
}
