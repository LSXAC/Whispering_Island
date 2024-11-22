using System;
using Godot;

public partial class MainMenu : Control
{
    PackedScene game = ResourceLoader.Load<PackedScene>("res://game_manager.tscn");

    public override void _Ready() { }

    public void OnNewGameButtoN()
    {
        SaveState.RemoveSave();

        Game_Manager gm = game.Instantiate() as Game_Manager;
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
