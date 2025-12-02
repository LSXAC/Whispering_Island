using System;
using Godot;

public partial class LauncherSave : Resource
{
    private static string save_path = "user://LauncherSave.tres";

    [Export]
    public string current_language = "en";

    [Export]
    public float master_volume = -15;

    [Export]
    public float music_volume = -5;

    [Export]
    public float sfx_volume = 5;

    [Export]
    public int window_mode = 0;

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
