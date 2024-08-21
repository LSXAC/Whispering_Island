using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class Islands_Manager : Node2D
{
    [Export]
    public island_menu island_menu;

    [Export]
    public Array<IslandSave> island_saves = new Array<IslandSave>();

    [Export]
    public Array<ResourceObjectManagerSave> roms = new Array<ResourceObjectManagerSave>();
    public static Islands_Manager INSTANCE = null;

    public override void _Ready()
    {
        INSTANCE = this;

        foreach (Island_Properties ip in GetChildren())
        {
            if (!roms.Contains(ip.ost.roms))
                roms.Add(ip.ost.roms);
        }
    }

    public override void _Process(double delta) { }

    public void LoadIslands(Array<ResourceObjectManagerSave> roms)
    {
        foreach (IslandSave is_save in island_saves)
        {
            foreach (Island_Properties ip in GetChildren())
            {
                if (ip.unique_island_id == is_save.start_unique_island_id)
                {
                    island_menu.CreateIsland(is_save.end_unique_island_id, is_save.dir, ip);
                }
            }
        }

        foreach (Island_Properties ip in GetChildren())
        {
            foreach (ResourceObjectManagerSave roms_t in roms)
                if (ip.unique_island_id == roms_t.unique_island_id)
                    ip.ost.LoadResourceObjects(roms_t);
        }
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
    }

    public void SaveIsland(Island_Properties.DIRECTION dir, int start_unique_id, int end_unique_id)
    {
        island_saves.Add(
            new IslandSave(dir: dir, start_unique_id: start_unique_id, end_unique_id: end_unique_id)
        );
        Debug.Print("new Island Saved");
        Debug.Print(island_saves.Count + " x");
    }
}
