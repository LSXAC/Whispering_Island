using System;
using Godot;

public partial class MainMenu : Control
{
    [Export]
    public CheckBox skip_tutorial;
    public static MainMenu INSTANCE;
    PackedScene game = ResourceLoader.Load<PackedScene>("res://game_manager.tscn");

    [Export]
    public VideoStreamPlayer intro_player;

    [Export]
    public VideoStreamPlayer background_player;

    [Export]
    public Control parent;

    [Export]
    public Button load_button;

    [Export]
    public bool skip_intro = false;

    public static LauncherSave launcherSave;

    public override void _Ready()
    {
        INSTANCE = this;
        if (!skip_intro)
        {
            intro_player.Play();
            intro_player.Finished += () => OpenMenu();
            parent.Visible = false;
        }
        else
            OpenMenu();

        if (!SaveState.HasSave())
            load_button.Disabled = true;

        if (LauncherSave.HasSave())
        {
            launcherSave = (LauncherSave)LauncherSave.LoadSave();
            TranslationServer.SetLocale(launcherSave.current_language);
            AudioServer.SetBusVolumeDb(
                AudioServer.GetBusIndex(SoundSlider.BUS.Master.ToString()),
                launcherSave.master_volume
            );
            AudioServer.SetBusVolumeDb(
                AudioServer.GetBusIndex(SoundSlider.BUS.Music.ToString()),
                launcherSave.music_volume
            );
            AudioServer.SetBusVolumeDb(
                AudioServer.GetBusIndex(SoundSlider.BUS.SFX.ToString()),
                launcherSave.sfx_volume
            );
            return;
        }

        TranslationServer.SetLocale("en");
        SaveLauncherConfig();
    }

    public static void SaveLauncherConfig()
    {
        launcherSave = new LauncherSave();

        launcherSave.current_language = TranslationServer.GetLocale();
        launcherSave.master_volume = AudioServer.GetBusVolumeDb(
            AudioServer.GetBusIndex(SoundSlider.BUS.Master.ToString())
        );
        launcherSave.music_volume = AudioServer.GetBusVolumeDb(
            AudioServer.GetBusIndex(SoundSlider.BUS.Music.ToString())
        );
        launcherSave.sfx_volume = AudioServer.GetBusVolumeDb(
            AudioServer.GetBusIndex(SoundSlider.BUS.SFX.ToString())
        );
        launcherSave.WriteSave();
    }

    public void OpenMenu()
    {
        parent.Visible = true;
        background_player.Play();
        intro_player.Stop();
        intro_player.Visible = false;
    }

    public void OnVisiblityChanged()
    {
        skip_tutorial.ButtonPressed = false;
        if (!SaveState.HasSave())
            load_button.Disabled = true;
        else
            load_button.Disabled = false;
    }

    public void OnNewGameButtoN()
    {
        SaveState.RemoveSave();

        Game_Manager gm = game.Instantiate() as Game_Manager;
        gm.new_game = true;
        if (skip_tutorial.ButtonPressed)
            gm.tutorial_finished = true;
        GetTree().Root.AddChild(gm);
        Visible = false;
    }

    public void OnLoadGameButtoN()
    {
        if (IsInstanceValid(Game_Manager.INSTANCE))
            Game_Manager.INSTANCE.QueueFree();

        Game_Manager gm = game.Instantiate() as Game_Manager;
        GetTree().Root.AddChild(gm);
        Visible = false;
    }

    public void OnExitGameButtoN()
    {
        GetTree().Quit();
    }
}
