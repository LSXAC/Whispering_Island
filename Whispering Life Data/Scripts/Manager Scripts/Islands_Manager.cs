using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class Islands_Manager : Node2D
{
    public static Islands_Manager INSTANCE = null;
    public Array<IslandSave> island_saves = new Array<IslandSave>();

    public Array<ResourceObjectManagerSave> roms = new Array<ResourceObjectManagerSave>();

    public Array<IslandBuildSave> build_saves = new Array<IslandBuildSave>();
    public int last_island_id;

    public override void _Ready()
    {
        INSTANCE = this;
        last_island_id = 0;
    }

    public Island_Properties GetNearestIsland(Vector2 pos)
    {
        int length = 200000;
        Island_Properties ips = null;
        foreach (Island_Properties ip in GetChildren())
        {
            if (pos.DistanceTo(ip.GlobalPosition) < length)
            {
                length = (int)pos.DistanceTo(ip.GlobalPosition);
                ips = ip;
            }
        }
        return ips;
    }

    public void LoadIslands(Array<ResourceObjectManagerSave> roms)
    {
        foreach (IslandSave is_save in island_saves)
        {
            foreach (Island_Properties ip in GetChildren())
                if (ip.matrix_island_id == is_save.matrix_island_id)
                    island_menu.instance.CreateIsland(is_save.island_id, is_save.dir, ip, true);
        }

        foreach (IslandBuildSave ibs in build_saves)
        {
            foreach (Island_Properties ip in GetChildren())
                if (ibs.matrix_island_id == ip.matrix_island_id)
                {
                    ip.building_manager.machine_saves = ibs.machine_saves;
                    ip.building_manager.belt_saves = ibs.belt_saves;
                    ip.building_manager.belt_transmitter_saves = ibs.belt_transmitter_saves;
                    ip.building_manager.placeable_saves = ibs.placeable_saves;
                    ip.building_manager.resource_obj_saves = ibs.resource_obj_saves;
                    ip.building_manager.belt_machine_saves = ibs.belt_machine_saves;
                    ip.building_manager.rail_saves = ibs.rail_saves;
                    ip.building_manager.LoadPlacedObjects();
                }
        }
    }

    public void RemoveIslandsThroughQuest()
    {
        if (island_saves.Count == 0)
            return;

        IslandSave i_s = island_saves[island_saves.Count - 1];

        foreach (IslandBuildSave ibs in build_saves)
            if (ibs.matrix_island_id == i_s.matrix_island_id)
            {
                build_saves.Remove(ibs);
                break;
            }

        foreach (ResourceObjectManagerSave roms_t in roms)
            if (roms_t.matrix_island_id == i_s.matrix_island_id)
            {
                roms.Remove(roms_t);
                break;
            }
        island_saves.RemoveAt(island_saves.Count - 1);

        Game_Manager.INSTANCE.SaveGame();
        GlobalFunctions.SetPlayerPositionToStart();
        Game_Manager.INSTANCE.save_state.WriteSave();
        GameMenu.INSTANCE.OnLoadButton();
    }

    public void SaveResourceObjects()
    {
        build_saves.Clear();
        foreach (Island_Properties ip in GetChildren())
        {
            IslandBuildSave ibs = new IslandBuildSave();
            ip.building_manager.SavePlacedObjects();
            ibs.matrix_island_id = ip.matrix_island_id;
            ibs.machine_saves = ip.building_manager.machine_saves;
            ibs.belt_saves = ip.building_manager.belt_saves;
            ibs.belt_transmitter_saves = ip.building_manager.belt_transmitter_saves;
            ibs.placeable_saves = ip.building_manager.placeable_saves;
            ibs.resource_obj_saves = ip.building_manager.resource_obj_saves;
            ibs.belt_machine_saves = ip.building_manager.belt_machine_saves;
            ibs.rail_saves = ip.building_manager.rail_saves;
            build_saves.Add(ibs);
        }
    }

    public void SaveIsland(Island_Properties.DIRECTION dir, int matrix_island_id, int island_id)
    {
        island_saves.Add(new IslandSave(dir: dir, matrix_island_id: matrix_island_id, island_id));
    }
}
