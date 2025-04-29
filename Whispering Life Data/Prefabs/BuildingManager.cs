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
    public Array<RailSave> rail_saves = new Array<RailSave>();

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

    public void LoadRails()
    {
        foreach (RailSave rails in rail_saves)
        {
            Rail temp =
                Database.GetBuildingType(Database.BUILDING_ID.RAIL).building_scene.Instantiate()
                as Rail;

            AddChild(temp);
            temp.Position = rails.position;
            temp.from_direction = rails.from_direction;
            temp.to_direction = rails.to_direction;
            temp.set_direction();
            temp.Set_Rotation(rails.current_rotation);
            if (rails.has_minecart)
            {
                Minecart cart =
                    Database
                        .GetBuildingType(Database.BUILDING_ID.MINECART)
                        .building_scene.Instantiate() as Minecart;
                cart.chestBase.chest_items = rails.chest_items;
                temp.item_holder.AddChild(cart);
                cart.Position = rails.minecart_position;
            }
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
            BeltItem item = (BeltItem)belt_item.Instantiate();

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
            Node2D belt =
                Database.GetBuildingType(belt_machine_save.id).building_scene.Instantiate() as Belt;

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
            placeable_building temp =
                Database.GetBuildingType(ps.building_id).building_scene.Instantiate()
                as placeable_building;

            if (temp == null)
                return;

            AddChild(temp);
            temp.Position = ps.pos;
        }
    }

    private MachineBase SelectSavedMachine(MachineSave machine_save)
    {
        MachineBase machine =
            Database.GetBuildingType(machine_save.building_id).building_scene.Instantiate()
            as MachineBase;
        if (machine != null)
            return machine;
        return null;
    }

    public void LoadPlacedObjects()
    {
        foreach (Node2D node in GetChildren())
            node.Free();

        LoadBelts();
        LoadMachines();
        LoadBeltTransmitter();
        LoadPlacableBuildings();
        LoadResources();
        LoadBeltMachines();
        LoadRails();
    }

    public void ClearAllObjects()
    {
        foreach (Node2D node in GetChildren())
            node.QueueFree();
    }

    public void SavePlacedObjects()
    {
        belt_saves.Clear();
        machine_saves.Clear();
        belt_transmitter_saves.Clear();
        placeable_saves.Clear();
        belt_machine_saves.Clear();
        resource_obj_saves.Clear();
        rail_saves.Clear();

        foreach (Node2D node in GetChildren())
        {
            if (node is ResourceObject)
            {
                resource_obj_saves.Add(((ResourceObject)node).SaveResourceObject());
                continue;
            }

            if (node is Rail)
            {
                RailSave rail_save = CreateRailSave((Rail)node);
                rail_saves.Add(rail_save);
                continue;
            }

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
                continue;
            }

            Debug.Print("Did not saved! - " + node.GetClass() + " | " + node.Name);
        }
    }

    public BeltSave CreateBeltSave(Belt belt)
    {
        BeltSave belt_save = new BeltSave(
            belt.Position,
            belt.from_direction,
            belt.to_direction,
            null,
            belt.current_rotation
        );

        if (belt.item_holder.hasBeltItem())
        {
            belt_save.holded_item = belt.item_holder.GetBeltItem().item.item_info;
            belt_save.beltItem_moving = belt.item_holder.moving_item;
            belt_save.beltItem_position = belt.item_holder.GetBeltItem().Position;
        }
        return belt_save;
    }

    public RailSave CreateRailSave(Rail rail)
    {
        bool has_minecart = false;
        Vector2 minecart_positon = Vector2.Zero;
        ItemSave[] chest_items = new ItemSave[20];
        if (rail.item_holder.GetChildCount() > 0)
        {
            has_minecart = true;
            minecart_positon = rail.item_holder.GetMinecart().Position;
            chest_items = rail.item_holder.GetMinecart().chestBase.chest_items;
        }

        RailSave rail_save = new RailSave(
            rail.Position,
            rail.from_direction,
            rail.to_direction,
            has_minecart,
            minecart_positon,
            chest_items,
            rail.current_rotation
        );
        return rail_save;
    }
}
