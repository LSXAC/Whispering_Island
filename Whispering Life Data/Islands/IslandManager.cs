using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class IslandManager : Node2D
{
    public static IslandManager instance = null;
    public Array<IslandSave> island_saves = new Array<IslandSave>();

    public Array<ResourceObjectManagerSave> roms = new Array<ResourceObjectManagerSave>();

    public Array<IslandBuildSave> build_saves = new Array<IslandBuildSave>();

    public int last_island_id;

    public int[] island_types_build = [0, 0, 0, 0, 0];

    public override void _Ready()
    {
        instance = this;
        island_types_build = [0, 0, 0, 0, 0];
        last_island_id = 0;
    }

    public Island GetNearestIsland(Vector2 pos)
    {
        int length = 200000;
        Island nearest_island = null;
        foreach (Island island in GetChildren())
        {
            if (pos.DistanceTo(island.GlobalPosition) < length)
            {
                length = (int)pos.DistanceTo(island.GlobalPosition);
                nearest_island = island;
            }
        }
        return nearest_island;
    }

    public void LoadIslands(Array<ResourceObjectManagerSave> roms)
    {
        foreach (IslandSave is_save in island_saves)
        {
            foreach (Island island in GetChildren())
                if (island.matrix_island_id == is_save.matrix_island_id)
                    IslandMenu.instance.CreateIsland(is_save.island_id, is_save.dir, island, true);
        }

        foreach (IslandBuildSave ibs in build_saves)
        {
            foreach (Island island in GetChildren())
                if (ibs.matrix_island_id == island.matrix_island_id)
                {
                    island.island_object_save_manager.machine_saves = ibs.machine_saves;
                    island.island_object_save_manager.belt_saves = ibs.belt_saves;
                    island.island_object_save_manager.belt_transmitter_saves =
                        ibs.belt_transmitter_saves;
                    island.island_object_save_manager.placeable_saves = ibs.placeable_saves;
                    island.island_object_save_manager.resource_obj_saves = ibs.resource_obj_saves;
                    island.island_object_save_manager.belt_machine_saves = ibs.belt_machine_saves;
                    island.island_object_save_manager.rail_saves = ibs.rail_saves;
                    island.island_object_save_manager.LoadPlacedObjects();
                    island.removable_objects_manager.removed_objects = ibs.removed_objects;
                    island.removable_objects_manager.RemoveSigns();
                }
        }
    }

    public void RemoveIslandsThroughQuest()
    {
        if (island_saves.Count == 0 || GetChildCount() <= 1)
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
        GameManager.instance.SaveGame();
        GlobalFunctions.SetPlayerPositionToStart();
        GameManager.instance.save_state.WriteSave();
        GameMenu.instance.OnLoadButton();
    }

    public Array<Island> GetIslands()
    {
        Array<Island> managers = new Array<Island>();
        foreach (Node node in GetChildren())
            if (node is Island island)
                managers.Add(island);
        return managers;
    }

    public void SaveResourceObjects()
    {
        build_saves.Clear();
        foreach (Island island in GetChildren())
        {
            IslandBuildSave ibs = new IslandBuildSave();
            island.island_object_save_manager.SavePlacedObjects();
            ibs.matrix_island_id = island.matrix_island_id;
            ibs.machine_saves = island.island_object_save_manager.machine_saves;
            ibs.belt_saves = island.island_object_save_manager.belt_saves;
            ibs.belt_transmitter_saves = island.island_object_save_manager.belt_transmitter_saves;
            ibs.placeable_saves = island.island_object_save_manager.placeable_saves;
            ibs.resource_obj_saves = island.island_object_save_manager.resource_obj_saves;
            ibs.belt_machine_saves = island.island_object_save_manager.belt_machine_saves;
            ibs.rail_saves = island.island_object_save_manager.rail_saves;
            ibs.removed_objects = island.removable_objects_manager.removed_objects;
            build_saves.Add(ibs);
        }
    }

    public void SaveIsland(Island.DIRECTION dir, int matrix_island_id, int island_id)
    {
        island_saves.Add(new IslandSave(dir: dir, matrix_island_id: matrix_island_id, island_id));
    }
}
