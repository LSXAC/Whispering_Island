using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class IslandObjectSaveManager : Node2D
{
    [Export]
    public Array<ResourceObjectSave> resource_obj_saves = new Array<ResourceObjectSave>();
    public Array<BeltSave> belt_saves = new Array<BeltSave>();
    public Array<BeltTransmitterSave> belt_transmitter_saves = new Array<BeltTransmitterSave>();
    public Array<MachineSave> machine_saves = new Array<MachineSave>();
    public Array<PlaceableSave> placeable_saves = new Array<PlaceableSave>();
    public Array<BeltMachineSave> belt_machine_saves = new Array<BeltMachineSave>();
    public Array<RailSave> rail_saves = new Array<RailSave>();

    private PackedScene belt_item_scene = ResourceLoader.Load<PackedScene>(ResourceUid.UidToPath("uid://dkue7sa7xyeyr")
    );

    public void LoadPlacedObjects()
    {
        foreach (Node2D node in this.GetChildren())
            node.Free();

        LoadBelts();
        LoadMachines();
        LoadBeltTransmitter();
        LoadPlacableBuildings();
        LoadResources();
        LoadBeltMachines();
        LoadRails();
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

        foreach (Node2D node in this.GetChildren())
        {
            if (node is MineableObject mineable_object)
            {
                resource_obj_saves.Add(mineable_object.Save());
                continue;
            }

            if (node is Rail rail)
            {
                rail_saves.Add((RailSave)rail.Save());
                continue;
            }

            if (node is Belt belt)
            {
                if (node is BeltSplitter splitter)
                {
                    belt_machine_saves.Add((BeltMachineSave)splitter.Save());
                    continue;
                }

                if (node is BeltCombiner combiner)
                {
                    belt_machine_saves.Add((BeltMachineSave)combiner.Save());
                    continue;
                }

                if (node is BeltTunnel belt_tunnel)
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
                                    bts.beltsave2 = (BeltSave)belt_tunnel.Save();
                                    placed = true;
                                    break;
                                }
                        }
                    if (placed)
                        continue;

                    belt_transmitter_saves.Add(
                        new BeltTransmitterSave(
                            (BeltSave)belt_tunnel.Save(),
                            ((BeltTunnel)node).is_tunnel_connected,
                            null
                        )
                    );
                }
                else
                    belt_saves.Add((BeltSave)belt.Save());
                continue;
            }

            if (node is MachineBase)
            {
                if (node is ProcessBuilding process_building)
                    machine_saves.Add((MachineSave)process_building.Save());

                if (node is ProductionMachine production_machine)
                    machine_saves.Add((MachineSave)production_machine.Save());

                if (node is RailStation rail_station)
                    machine_saves.Add((MachineSave)rail_station.Save());

                if (node is ChestBase chest_base)
                    machine_saves.Add((MachineSave)chest_base.Save());

                if (node is Trashcan trash_can)
                    machine_saves.Add((MachineSave)trash_can.Save());
                continue;
            }

            if (node is placeable_building placeable_building)
            {
                try
                {
                    placeable_saves.Add((PlaceableSave)placeable_building.Save());
                }
                catch (NotImplementedException ex)
                {
                    Debug.Print(node.GetClass() + ex.StackTrace);
                }
                continue;
            }

            Debug.Print("Did not saved! - " + node.GetClass() + " | " + node.Name);
        }
    }

    public void LoadBelts()
    {
        foreach (BeltSave belt_save in belt_saves)
        {
            Belt belt =
                Database
                    .GetBuildingMenuListChildObjectInfo(Database.BUILDING_ID.BELT)
                    .scene.Instantiate() as Belt;

            AddChild(belt);
            belt.Load(belt_save);
        }
    }

    public void LoadRails()
    {
        foreach (RailSave rails in rail_saves)
        {
            Rail rail =
                Database
                    .GetBuildingMenuListChildObjectInfo(Database.BUILDING_ID.RAIL)
                    .scene.Instantiate() as Rail;

            AddChild(rail);
            rail.Load(rails);
        }
    }

    public void LoadResources()
    {
        foreach (ResourceObjectSave ros in resource_obj_saves)
        {
            MineableObject mineable_object =
                Database.GetBuildingMenuListChildObjectInfo(ros.building_id).scene.Instantiate()
                as MineableObject;
            AddChild(mineable_object);
            mineable_object.Load(ros);
        }
    }

    public void LoadBeltTransmitter()
    {
        foreach (BeltTransmitterSave bts in belt_transmitter_saves)
        {
            BeltTunnel belt_tunnel =
                Database
                    .GetBuildingMenuListChildObjectInfo(Database.BUILDING_ID.BELT_TUNNEL)
                    .scene.Instantiate() as BeltTunnel;

            AddChild(belt_tunnel);
            belt_tunnel.Load(bts.beltsave1);

            if (bts.is_connected)
            {
                BeltTunnel belt_tunnel_2 =
                    Database
                        .GetBuildingMenuListChildObjectInfo(Database.BUILDING_ID.BELT_TUNNEL)
                        .scene.Instantiate() as BeltTunnel;

                AddChild(belt_tunnel_2);
                belt_tunnel_2.Load(bts.beltsave2);

                belt_tunnel.is_tunnel_connected = true;
                belt_tunnel_2.is_tunnel_connected = true;
                belt_tunnel.connected_itemholder = belt_tunnel_2.item_holder;
                belt_tunnel_2.connected_itemholder = belt_tunnel.item_holder;
            }
        }
    }

    public void LoadBeltMachines()
    {
        foreach (BeltMachineSave belt_machine_save in belt_machine_saves)
        {
            Node2D belt =
                Database
                    .GetBuildingMenuListChildObjectInfo(belt_machine_save.id)
                    .scene.Instantiate() as Belt;

            if (belt == null)
                return;

            AddChild(belt);

            if (belt is BeltSplitter splitter)
                splitter.Load(belt_machine_save);

            if (belt is BeltCombiner combiner)
                combiner.Load(belt_machine_save);
        }
    }

    public void LoadMachines()
    {
        foreach (MachineSave machine_save in machine_saves)
        {
            MachineBase temp = SelectSavedMachine(machine_save);

            if (temp == null)
                return;

            AddChild(temp);

            if (temp is ProcessBuilding process_building)
                process_building.Load(machine_save);

            if (temp is ProductionMachine production_machine)
                production_machine.Load(machine_save);

            if (temp is RailStation rail_station)
                rail_station.Load(machine_save);

            if (temp is ChestBase chest_base)
                chest_base.Load(machine_save);
        }
    }

    public void LoadPlacableBuildings()
    {
        foreach (PlaceableSave ps in placeable_saves)
        {
            placeable_building placeable =
                Database.GetBuildingMenuListChildObjectInfo(ps.building_id).scene.Instantiate()
                as placeable_building;

            if (placeable == null)
                return;

            AddChild(placeable);
            // TODO:
            placeable.Position = ps.pos; //placeable is abstract, no implementation of save/load
        }
    }

    private MachineBase SelectSavedMachine(MachineSave machine_save)
    {
        MachineBase machine =
            Database
                .GetBuildingMenuListChildObjectInfo(machine_save.building_id)
                .scene.Instantiate() as MachineBase;
        if (machine != null)
            return machine;
        return null;
    }

    public void ClearAllObjects()
    {
        foreach (Node2D node in GetChildren())
            node.QueueFree();
    }
}
