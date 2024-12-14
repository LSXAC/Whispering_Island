using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class Islands_Manager : Node2D
{
    [Export]
    public Array<IslandSave> island_saves = new Array<IslandSave>();

    [Export]
    public Array<ResourceObjectManagerSave> roms = new Array<ResourceObjectManagerSave>();
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

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("RemoveIsland"))
            RemoveIslands();
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
                    ip.ost.LoadResourceObjects(roms_t);
        }
    }

    public void RemoveIslands()
    {
        if (island_saves.Count == 0)
            return;

        island_saves.RemoveAt(island_saves.Count - 1);
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
    }

    public void SaveIsland(Island_Properties.DIRECTION dir, int matrix_island_id, int island_id)
    {
        island_saves.Add(new IslandSave(dir: dir, matrix_island_id: matrix_island_id, island_id));
        Debug.Print("new Island Saved");
        Debug.Print(island_saves.Count + " x");
    }
}
