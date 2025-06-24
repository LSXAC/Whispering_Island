using Godot;

public partial class MainMenu : Control
{
    [Export]
    public CheckBox skip_tutorial;

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

    public static MainMenu instance = null;
    public static LauncherSave launcherSave;
    PackedScene game = ResourceLoader.Load<PackedScene>("res://Scenes/Manager/game_manager.tscn");

    public override void _Ready()
    {
        instance = this;
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

    public async void OnNewGameButtoN()
    {
        SaveState.RemoveSave();

        TransitionManager.StartTransition();
        await TransitionManager.IsInTransitionLoop();

        GameManager gm = game.Instantiate() as GameManager;
        gm.new_game = true;
        if (skip_tutorial.ButtonPressed)
            gm.tutorial_finished = true;
        GetTree().Root.AddChild(gm);
        GetTree().Root.MoveChild(gm, 0);

        TransitionManager.instance.StopTransition();
        parent.Visible = false;
    }

    public async void OnLoadGameButtoN()
    {
        TransitionManager.StartTransition();
        await TransitionManager.IsInTransitionLoop();

        if (IsInstanceValid(GameManager.instance))
            GameManager.instance.QueueFree();

        LoadGame();

        TransitionManager.instance.StopTransition();
        parent.Visible = false;
    }

    public void LoadGame()
    {
        if (IsInstanceValid(GameManager.instance))
            GameManager.instance.QueueFree();

        GameManager gm = game.Instantiate() as GameManager;
        GetTree().Root.AddChild(gm);
    }

    public void OnExitGameButtoN()
    {
        GetTree().Quit();
    }
}
