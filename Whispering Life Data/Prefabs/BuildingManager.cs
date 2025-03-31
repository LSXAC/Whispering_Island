using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class BuildingManager : Node2D
{
    public Array<BeltSave> belt_saves = new Array<BeltSave>();
    public Array<BeltTransmitterSave> belt_transmitter_saves = new Array<BeltTransmitterSave>();
    public Array<MachineSave> machine_saves = new Array<MachineSave>();
    public Array<PlaceableSave> placeable_saves = new Array<PlaceableSave>();
    public Array<BeltMachineSave> belt_machine_saves = new Array<BeltMachineSave>();

    [Export]
    public Array<ResourceObjectSave> resource_obj_saves = new Array<ResourceObjectSave>();
    private PackedScene belt_item = ResourceLoader.Load<PackedScene>("res://belt_item.tscn");

    public void LoadBelts()
    {
        foreach (BeltSave belt_save in belt_saves)
        {
            Belt temp =
                Database.GetBuildingType(Database.BUILDING_ID.BELT).building_scene.Instantiate()
                as Belt;

            InitBelt(temp, belt_save);
            InitBeltItem(temp, belt_save);
        }
    }

    public void LoadResources()
    {
        foreach (ResourceObjectSave ros in resource_obj_saves)
        {
            ResourceObject temp =
                Database.GetBuildingType(ros.building_id).building_scene.Instantiate()
                as ResourceObject;
            AddChild(temp);
            temp.ResourceObjectLoad(ros);
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
        AddChild(belt);
        belt.Position = beltsave.position;
        belt.from_direction = beltsave.from_direction;
        belt.to_direction = beltsave.to_direction;
        belt.set_direction();
        belt.Set_Rotation(beltsave.current_rotation);
    }

    public void InitBeltItem(Belt belt, BeltSave beltsave)
    {
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
                InitBeltItem(temp, bts.beltsave1);

                BeltTunnel temp2 =
                    Database
                        .GetBuildingType(Database.BUILDING_ID.BELTTUNNEL)
                        .building_scene.Instantiate() as BeltTunnel;

                InitBelt(temp2, bts.beltsave2);
                InitBeltItem(temp, bts.beltsave2);

                temp.is_tunnel_connected = true;
                temp.Name = "BeltTunnel1";
                temp2.is_tunnel_connected = true;
                temp2.Name = "BeltTunnel2";
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
                InitBeltItem(temp, bts.beltsave1);
            }
        }
    }

    public void LoadBeltMachines()
    {
        foreach (BeltMachineSave belt_machine_save in belt_machine_saves)
        {
            Node2D belt = null;

            if (belt_machine_save.id == Database.BUILDING_ID.BELT_COMBINER_2x1)
                belt =
                    Database
                        .GetBuildingType(Database.BUILDING_ID.BELT_COMBINER_2x1)
                        .building_scene.Instantiate() as BeltCombiner;

            if (belt_machine_save.id == Database.BUILDING_ID.BELT_COMBINER_3x1)
                belt =
                    Database
                        .GetBuildingType(Database.BUILDING_ID.BELT_COMBINER_3x1)
                        .building_scene.Instantiate() as BeltCombiner;

            if (belt_machine_save.id == Database.BUILDING_ID.BELT_SPLITTER_1x2)
                belt =
                    Database
                        .GetBuildingType(Database.BUILDING_ID.BELT_SPLITTER_1x2)
                        .building_scene.Instantiate() as BeltSplitter;

            if (belt_machine_save.id == Database.BUILDING_ID.BELT_SPLITTER_1x3)
                belt =
                    Database
                        .GetBuildingType(Database.BUILDING_ID.BELT_SPLITTER_1x3)
                        .building_scene.Instantiate() as BeltSplitter;

            if (belt == null)
                return;

            AddChild(belt);
            belt.Position = belt_machine_save.b1.position;

            if (belt is BeltSplitter splitter)
            {
                InitBeltItem(splitter, belt_machine_save.b1);
                InitBeltItem(splitter.belt_0, belt_machine_save.b2);
                InitBeltItem(splitter.belt_1, belt_machine_save.b3);
                if (belt_machine_save.b4 != null)
                    InitBeltItem(splitter.belt_2, belt_machine_save.b4);
            }
            if (belt is BeltCombiner combiner)
            {
                InitBeltItem(combiner, belt_machine_save.b1);
                InitBeltItem(combiner.belt_0, belt_machine_save.b2);
                InitBeltItem(combiner.belt_1, belt_machine_save.b3);
                if (belt_machine_save.b4 != null)
                    InitBeltItem(combiner.belt_2, belt_machine_save.b4);
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

            AddChild(temp);
            temp.Position = machine_save.pos;
            temp.Scale = machine_save.scale;
            temp.machine_enabled = machine_save.machine_enabled;

            if (temp is ProcessBuilding)
            {
                for (int i = 0; i < machine_save.furnace_slots.Length; i++)
                    ((ProcessBuilding)temp).item_array[i] = machine_save.furnace_slots[i];

                ((ProcessBuilding)temp).fuel_left = machine_save.fuel_left;
            }

            if (temp is ChestBase)
            {
                for (int i = 0; i < machine_save.chest_items.Length; i++)
                    ((ChestBase)temp).chest_items[i] = machine_save.chest_items[i];
            }
            if (temp is Trashcan)
            {
                for (int i = 0; i < machine_save.chest_items.Length; i++)
                    ((ChestBase)temp).chest_items[i] = machine_save.chest_items[i];
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

            AddChild(temp);
            temp.Position = ps.pos;
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
            return temp;
        }

        if (machine_save.building_id == Database.BUILDING_ID.CHEST)
            return Database.GetBuildingType(Database.BUILDING_ID.CHEST).building_scene.Instantiate()
                as ChestBase;

        if (machine_save.building_id == Database.BUILDING_ID.TRASHCAN)
            return Database
                    .GetBuildingType(Database.BUILDING_ID.TRASHCAN)
                    .building_scene.Instantiate() as Trashcan;

        return null;
    }

    public void LoadPlacedObjects()
    {
        foreach (Node2D node in GetChildren())
            node.QueueFree();

        LoadBelts();
        LoadMachines();
        LoadBeltTransmitter();
        LoadPlacableBuildings();
        LoadResources();
        LoadBeltMachines();
    }

    public void SavePlacedObjects()
    {
        belt_saves.Clear();
        machine_saves.Clear();
        belt_transmitter_saves.Clear();
        placeable_saves.Clear();
        belt_machine_saves.Clear();
        resource_obj_saves.Clear();

        foreach (Node2D node in GetChildren())
        {
            if (node is ResourceObject)
                resource_obj_saves.Add(((ResourceObject)node).SaveResourceObject());

            if (node is Belt)
            {
                BeltSave belt_save = CreateBeltSave((Belt)node);

                if (node is BeltSplitter splitter)
                {
                    BeltMachineSave belt_machine = new BeltMachineSave();
                    belt_machine.id = splitter.building_id;
                    belt_machine.b1 = belt_save;
                    belt_machine.b2 = CreateBeltSave(splitter.belt_0);
                    belt_machine.b3 = CreateBeltSave(splitter.belt_1);
                    if (splitter.belt_2 != null)
                        belt_machine.b4 = CreateBeltSave(splitter.belt_2);
                    belt_machine_saves.Add(belt_machine);
                    continue;
                }

                if (node is BeltCombiner combiner)
                {
                    BeltMachineSave belt_machine = new BeltMachineSave();
                    belt_machine.id = combiner.building_id;
                    belt_machine.b1 = belt_save;
                    belt_machine.b2 = CreateBeltSave(combiner.belt_0);
                    belt_machine.b3 = CreateBeltSave(combiner.belt_1);
                    if (combiner.belt_2 != null)
                        belt_machine.b4 = CreateBeltSave(combiner.belt_2);
                    belt_machine_saves.Add(belt_machine);
                    continue;
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
                    for (int i = 0; i < 3; i++)
                        ms.furnace_slots[i] = ((ProcessBuilding)node).item_array[i];
                    ms.fuel_left = ((ProcessBuilding)node).fuel_left;
                }

                if (node is ChestBase || node is Trashcan)
                    ms.chest_items = ((ChestBase)node).chest_items;

                machine_saves.Add(ms);
                continue;
            }

            if (node is placeable_building)
            {
                PlaceableSave ps = new PlaceableSave(
                    ((placeable_building)node).building_id,
                    node.Position
                );
                placeable_saves.Add(ps);
            }
        }
    }

    public BeltSave CreateBeltSave(Belt belt)
    {
        BeltSave belt_save = new BeltSave(
            belt.Position,
            (belt).from_direction,
            (belt).to_direction,
            null,
            (belt).current_rotation
        );

        if ((belt).item_holder.hasBeltItem())
        {
            belt_save.holded_item = (belt).item_holder.GetBeltItem().item.item_info;
            belt_save.beltItem_moving = (belt).item_holder.moving_item;
            belt_save.beltItem_position = (belt).item_holder.GetBeltItem().Position;
        }
        return belt_save;
    }
}
