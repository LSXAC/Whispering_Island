using System;
using Godot;

public partial class MainMenu : Control
{
    [Export]
    public PackedScene game;

    public override void _Ready() { }

    public void OnNewGameButtoN()
    {
        GetTree().ChangeSceneToPacked(game);
    }

    public void OnLoadGameButtoN()
    {
        GetTree().ChangeSceneToPacked(game);
    }

    public void OnExitGameButtoN()
    {
        GetTree().Quit();
    }
}
