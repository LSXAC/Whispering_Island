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
    public StyleBoxFlat StyleBoxRespawn { get; set; }

    [Export]
    public StyleBoxFlat StyleBoxCooldown { get; set; }
    Action action = null;

    public override void _Ready()
    {
        Value = 0;
        MaxValue = 100;
        timer = GetNode<Timer>("Timer");
        Visible = false;
    }

    public void InitTimer(
        double max_seconds,
        STATE new_state,
        string start_string,
        Action action = null
    )
    {
        // Assign action to be called on timer completion
        this.action = action;
        MaxValue = max_seconds;
        Value = 0;
        timer.WaitTime = 1;
        if (new_state != STATE.COOLDOWN)
            Visible = true;
        if (max_seconds < 1)
            timer.WaitTime = max_seconds;
        current_state = new_state;
        AddThemeStyleboxOverride("fill", StyleBoxRespawn);
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

        if (hover_menu.instance.current_object == parent)
            hover_menu.InitHoverMenu(parent);

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

    public int GetTimeLeft()
    {
        return Mathf.CeilToInt(MaxValue - Value);
    }
}
