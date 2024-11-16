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
    public QuestSave quest_save = new QuestSave();

    [Export]
    public Array<BeltSave> belt_saves = new Array<BeltSave>();

    [Export]
    public Array<BeltTransmitterSave> belt_transmitter_saves = new Array<BeltTransmitterSave>();

    [Export]
    public Array<MachineSave> machine_saves = new Array<MachineSave>();

    [Export]
    public Array<PlaceableSave> placeable_saves = new Array<PlaceableSave>();

    [Export]
    public bool tutorial_finished = false;

    [Export]
    public string current_language = "en";

    [Export]
    public float master_volume = -15;

    [Export]
    public float music_volume = -5;

    [Export]
    public float sfx_volume = 5;

    [Export]
    public float game_time_since_start = 0;

    [Export]
    public string dateTime_save_string;

    [Export]
    public Dictionary<Inventory.ITEM_ID, ResearchSave> research_saves =
        new Dictionary<Inventory.ITEM_ID, ResearchSave>();

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
}
