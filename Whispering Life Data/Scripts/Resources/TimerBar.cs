using System;
using Godot;

public partial class TimerBar : ProgressBar
{
    public enum state
    {
        NONE,
        RESPAWNING,
        COOLDOWN
    };

    Timer timer;
    Label label;

    public Node2D parent;
    public state currentstate = state.NONE;

    StyleBoxFlat styleBoxRespawn = new StyleBoxFlat();
    StyleBoxFlat styleBoxCooldown = new StyleBoxFlat();

    public override void _Ready()
    {
        styleBoxRespawn.BgColor = new Color(1, 0, 0, 1);
        styleBoxCooldown.BgColor = new Color(0.3f, 0.3f, 0.3f, 1);
        timer = GetNode<Timer>("Timer");
        label = GetNode<Label>("Label");
        Visible = false;
    }

    public void InitTimer(double max_seconds, state new_state)
    {
        MaxValue = max_seconds;
        Value = max_seconds;
        if (max_seconds < 1)
            timer.WaitTime = max_seconds;
        else
            timer.WaitTime = 1;
        currentstate = new_state;
        //if (currentstate == state.COOLDOWN)
        // AddThemeStyleboxOverride("background", styleBoxCooldown);
        //else
        AddThemeStyleboxOverride("background", styleBoxRespawn);
        if (new_state != state.COOLDOWN)
            Visible = true;
        UpdateLabel();
        timer.Start();
    }

    public void OnTimerTimeout()
    {
        Value--;
        UpdateLabel();
        if (Value <= 0)
        {
            timer.Stop();
            Visible = false;
            if (parent is ResourceObject)
                ((ResourceObject)parent).Reset();
            return;
        }
    }

    private void UpdateLabel()
    {
        label.Text = Value + "";
    }
}
