using System;
using System.Runtime.CompilerServices;
using Godot;

public partial class SaveState : Resource
{
    [Export]
    public CharacterSave char_save = new CharacterSave();

    [Export]
    public EnvironmentSave env_save = new EnvironmentSave();

    [Export]
    public QuestSave quest_save = new QuestSave();

    public float game_time_since_start = 0;

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
        return ResourceLoader.Load(save_path);
    }
}
