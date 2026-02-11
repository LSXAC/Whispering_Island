using System;
using System.Diagnostics;
using Godot;

public partial class HeartManager : ColorRect
{
    public static HeartManager instance = null;
    public int current_hearts = 3;
    public int max_hearts = 3;

    [Export]
    public HBoxContainer parent;

    public override void _Ready()
    {
        instance = this;
    }

    public void RemoveHeart()
    {
        current_hearts -= 1;
        UpdateHeartUI();

        if (current_hearts <= 0)
            GameManager.instance.GameOver();
    }

    public void AddHeart()
    {
        current_hearts += 1;
        if (current_hearts > max_hearts)
            current_hearts = max_hearts;

        UpdateHeartUI();
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
}
