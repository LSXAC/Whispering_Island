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

    StyleBoxFlat styleBoxRespawn = new StyleBoxFlat();
    StyleBoxFlat styleBoxCooldown = new StyleBoxFlat();
    Action action = null;

    public override void _Ready()
    {
        styleBoxRespawn.BgColor = new Color(1, 0, 0, 1);
        styleBoxCooldown.BgColor = new Color(0.3f, 0.3f, 0.3f, 1);
        timer = GetNode<Timer>("Timer");
        label = GetNode<Label>("Label");
        Visible = false;
    }

    public void InitTimer(double max_seconds, STATE new_state, Action action = null)
    {
        // Assign action to be called on timer completion
        this.action = action;

        MaxValue = max_seconds;
        Value = max_seconds;
        if (max_seconds < 1)
            timer.WaitTime = max_seconds;
        else
            timer.WaitTime = 1;
        current_state = new_state;
        AddThemeStyleboxOverride("background", styleBoxRespawn);
        if (new_state != STATE.COOLDOWN)
            Visible = true;
        UpdateLabel();
        timer.Start();
        Debug.Print(
            "Timer started with max_seconds: " + max_seconds + " and state: " + new_state.ToString()
        );
    }

    public void OnTimerTimeout()
    {
        if (parent == null)
            GD.PrintErr("TimerBar parent is null!");

        Value--;
        // Invoke the assigned action if any
        if (action != null)
            action.Invoke();

        UpdateLabel();
        if (Value <= 0)
        {
            timer.Stop();
            Visible = false;
            if (parent is MineableObject)
                ((MineableObject)parent).Reset();
            return;
        }
    }

    public double GetInvertedProgressPercent()
    {
        return 1.0 - (Value / MaxValue);
    }

    private void UpdateLabel()
    {
        label.Text = Value + "";
    }
}
