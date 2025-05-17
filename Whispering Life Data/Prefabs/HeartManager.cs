using System;
using System.Diagnostics;
using Godot;

public partial class HeartManager : Panel
{
    public static HeartManager INSTANCE = null;
    public int current_hearts = 3;
    public int max_hearts = 3;

    [Export]
    public HBoxContainer parent;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public void RemoveHeart()
    {
        current_hearts -= 1;
        UpdateHeartUI();

        if (current_hearts <= 0)
            Game_Manager.INSTANCE.GameOver();
    }

    public void UpdateHeartUI()
    {
        if (current_hearts <= 0)
            return;

        foreach (TextureRect tr in parent.GetChildren())
            tr.Visible = true;

        for (int i = Math.Abs(current_hearts - 3); i > 0; i--)
            ((TextureRect)parent.GetChild(i)).Visible = false;
    }

    public void AddHeart()
    {
        throw new NotImplementedException();
    }
}
