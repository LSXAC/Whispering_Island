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
    public int money = 0;

    [Export]
    public bool is_doubled_quest = false;

    [Export]
    public int Research_Points = 0;

    [Export]
    public bool tutorial_finished = false;

    [Export]
    public string dateTime_save_string;

    [Export]
    public int current_days = 0;

    [Export]
    public int current_game_time = 0;

    [Export]
    public Dictionary<Inventory.ITEM_ID, ResearchSave> research_saves =
        new Dictionary<Inventory.ITEM_ID, ResearchSave>();

    [Export]
    public bool in_research = false;

    [Export]
    public Inventory.ITEM_ID current_selected_item_id = Inventory.ITEM_ID.NULL;

    [Export]
    public int current_research_prog = 0;

    [Export]
    public int selected_sub_id = -1;

    [Export]
    public Dictionary<Inventory.ITEM_ID, bool> discovered_items =
        new Dictionary<Inventory.ITEM_ID, bool>();

    [Export]
    public int[] skill_saves = new int[4];

    [Export]
    public int[] island_types_build = new int[4];

    [Export]
    public float difficulty_multiplier = 1.0f;

    [Export]
    public GameManager.DIFFICULTY difficulty = GameManager.DIFFICULTY.NORMAL;

    [Export]
    public float mood = 0.8f;

    [Export]
    public float stability = 1.0f;

    [Export]
    public float current_magic_power = 0.0f;

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
