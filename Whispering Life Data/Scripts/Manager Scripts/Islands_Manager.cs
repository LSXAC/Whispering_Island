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
    public int last_island_id = 0;

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
                Debug.Print(ip.matrix_island_id + " | " + is_save.matrix_island_id);
                if (ip.matrix_island_id == is_save.matrix_island_id)
                {
                    island_menu.CreateIsland(is_save.island_id, is_save.dir, ip);
                }
            }
        }

        Debug.Print(roms.Count + " !!!!!!");
        foreach (ResourceObjectManagerSave roms_t in roms)
        {
            foreach (Island_Properties ip in GetChildren())
                if (ip.matrix_island_id == roms_t.matrix_island_id)
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

    public void SaveIsland(Island_Properties.DIRECTION dir, int matrix_island_id, int island_id)
    {
        island_saves.Add(new IslandSave(dir: dir, matrix_island_id: matrix_island_id, island_id));
        Debug.Print("new Island Saved");
        Debug.Print(island_saves.Count + " x");
    }
}
