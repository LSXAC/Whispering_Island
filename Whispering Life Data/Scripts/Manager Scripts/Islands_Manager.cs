using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class Islands_Manager : Node2D
{
    public Array<IslandSave> island_saves = new Array<IslandSave>();

    public Array<ResourceObjectManagerSave> roms = new Array<ResourceObjectManagerSave>();

    public Array<IslandBuildSave> build_saves = new Array<IslandBuildSave>();
    public static Islands_Manager INSTANCE = null;
    public int last_island_id;

    public override void _Ready()
    {
        INSTANCE = this;
        last_island_id = 0;

        foreach (Island_Properties ip in GetChildren())
        {
            if (!roms.Contains(ip.ost.roms))
                roms.Add(ip.ost.roms);
        }
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
                Debug.Print(ip.Name + " | " + length);
                ips = ip;
            }
        }
        Debug.Print(ips.Name);
        return ips;
    }

    public void LoadIslands(Array<ResourceObjectManagerSave> roms)
    {
        Debug.Print(island_saves.Count + "");
        foreach (IslandSave is_save in island_saves)
        {
            foreach (Island_Properties ip in GetChildren())
            {
                Debug.Print(ip.matrix_island_id + " | " + is_save.matrix_island_id);
                if (ip.matrix_island_id == is_save.matrix_island_id)
                {
                    island_menu.instance.CreateIsland(is_save.island_id, is_save.dir, ip, true);
                }
            }
        }

        Debug.Print(roms.Count + " !!!!!!");
        foreach (ResourceObjectManagerSave roms_t in roms)
        {
            foreach (Island_Properties ip in GetChildren())
                if (ip.matrix_island_id == roms_t.matrix_island_id)
                {
                    ip.ost.LoadResourceObjects(roms_t);
                }
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
                    ip.building_manager.LoadPlacedObjects();
                }
        }
    }

    public void RemoveIslands()
    {
        if (island_saves.Count == 0)
            return;

        Debug.Print("IS: STATE 1");
        IslandSave i_s = island_saves[island_saves.Count - 1];
        Debug.Print("IS: STATE 2");

        foreach (IslandBuildSave ibs in build_saves)
            if (ibs.matrix_island_id == i_s.matrix_island_id)
            {
                build_saves.Remove(ibs);
                break;
            }

        Debug.Print("IS: STATE 3");
        foreach (ResourceObjectManagerSave roms_t in roms)
            if (roms_t.matrix_island_id == i_s.matrix_island_id)
            {
                roms.Remove(roms_t);
                break;
            }
        Debug.Print("IS: STATE 4");
        island_saves.RemoveAt(island_saves.Count - 1);
        Debug.Print("IS: STATE 5");
        Game_Manager.INSTANCE.SaveGame();
        Game_Manager.INSTANCE.save_state.char_save.player_position = new Vector2(10f, -170f);
        Game_Manager.INSTANCE.save_state.WriteSave();
        GameMenu.INSTANCE.OnLoadButton();
    }

    public void SaveResourceObjects()
    {
        foreach (Island_Properties ip in GetChildren())
            ip.ost.SaveResourseObjects();

        foreach (Island_Properties ip in GetChildren())
        {
            if (!roms.Contains(ip.ost.roms))
                roms.Add(ip.ost.roms);
        }

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
            build_saves.Add(ibs);
        }
    }

    public void SaveIsland(Island_Properties.DIRECTION dir, int matrix_island_id, int island_id)
    {
        island_saves.Add(new IslandSave(dir: dir, matrix_island_id: matrix_island_id, island_id));
        Debug.Print("new Island Saved");
        Debug.Print(island_saves.Count + " x");
    }
}
