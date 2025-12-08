using System;
using System.Diagnostics;
using Godot;

public partial class TimerBar : ProgressBar
{
    public enum STATE
    {
        NONE,
        SPAWNING,
        COOLDOWN
    };

    Timer timer;
    Label label;

    public Node2D parent;
    public STATE current_state = STATE.NONE;

    [Export]
    StyleBoxFlat styleBoxRespawn = new StyleBoxFlat();

    [Export]
    StyleBoxFlat styleBoxCooldown = new StyleBoxFlat();
    Action action = null;

    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        label = GetNode<Label>("Label");
        label.Text = "";
        Visible = false;
    }

    public void InitTimer(double max_seconds, STATE new_state, Action action = null)
    {
        // Assign action to be called on timer completion
        this.action = action;
        Visible = true;
        MaxValue = max_seconds;
        Value = 0;
        timer.WaitTime = 1;
        if (max_seconds < 1)
            timer.WaitTime = max_seconds;
        current_state = new_state;
        AddThemeStyleboxOverride("fill", styleBoxRespawn);
        timer.Start();
        Debug.Print(
            "Timer started with max_seconds: " + max_seconds + " and state: " + new_state.ToString()
        );
    }

    public void OnTimerTimeout()
    {
        if (parent == null)
            GD.PrintErr("TimerBar parent is null!");

        Value++;
        // Invoke the assigned action if any
        if (action != null)
            action.Invoke();

        if (Value >= MaxValue)
        {
            timer.Stop();
            Visible = false;
            if (parent is MineableObject)
                ((MineableObject)parent).Reset();
            return;
        }
    }

    public double GetProgressPercent()
    {
        return Value / MaxValue;
    }

    public void UpdateLabel(string text)
    {
        label.Text = text;
    }
}
