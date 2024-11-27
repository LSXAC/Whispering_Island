using System;
using Godot;

public partial class MainMenu : Control
{
    [Export]
    public CheckBox skip_tutorial;
    PackedScene game = ResourceLoader.Load<PackedScene>("res://game_manager.tscn");

    public override void _Ready() { }

    public void OnVisiblityChanged()
    {
        skip_tutorial.ButtonPressed = false;
    }

    public void OnNewGameButtoN()
    {
        SaveState.RemoveSave();

        Game_Manager gm = game.Instantiate() as Game_Manager;
        if (skip_tutorial.ButtonPressed)
            Game_Manager.tutorial_finished = true;
        GetTree().Root.AddChild(gm);
        Visible = false;
    }

    public void OnLoadGameButtoN()
    {
        GetTree().ChangeSceneToFile("res://game_manager.tscn");
    }

    public void OnExitGameButtoN()
    {
        GetTree().Quit();
    }
}
