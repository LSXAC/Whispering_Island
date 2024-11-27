using System;
using Godot;

public partial class MainMenu : Control
{
    [Export]
    public CheckBox skip_tutorial;
    public static MainMenu INSTANCE;
    PackedScene game = ResourceLoader.Load<PackedScene>("res://game_manager.tscn");

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public void OnVisiblityChanged()
    {
        skip_tutorial.ButtonPressed = false;
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
        Game_Manager gm = game.Instantiate() as Game_Manager;
        GetTree().Root.AddChild(gm);
        Visible = false;
    }

    public void OnExitGameButtoN()
    {
        GetTree().Quit();
    }
}
