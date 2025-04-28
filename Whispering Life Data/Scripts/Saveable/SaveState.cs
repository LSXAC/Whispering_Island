using System;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;

public partial class SaveState : Resource
{
    [Export]
    public CharacterSave char_save = new CharacterSave();

    [Export]
    public EnvironmentSave env_save = new EnvironmentSave();

    [Export]
    public Array<IslandBuildSave> build_saves = new Array<IslandBuildSave>();

    [Export]
    public QuestSave quest_save = new QuestSave();

    [Export]
    public int current_hearts = 3;

    [Export]
    public bool is_doubled_quest = false;

    [Export]
    public int Research_Points = 0;

    [Export]
    public bool tutorial_finished = false;

    [Export]
    public float game_time_since_start = 0;

    [Export]
    public string dateTime_save_string;

    [Export]
    public Dictionary<InventoryBase.ITEM_ID, ResearchSave> research_saves =
        new Dictionary<InventoryBase.ITEM_ID, ResearchSave>();

    [Export]
    public int[] skill_saves = new int[4];

    private static string save_path = "user://save.tres";
    public static string game_version = "a.0.1";

    public void WriteSave()
    {
        ResourceSaver.Save(this, save_path);
    }

    public static bool HasSave()
    {
        return ResourceLoader.Exists(save_path);
    }

    public static Resource LoadSave()
    {
        return ResourceLoader.Load(save_path, "", ResourceLoader.CacheMode.Replace);
    }

    public static void RemoveSave()
    {
        DirAccess.RemoveAbsolute(save_path);
    }
}
