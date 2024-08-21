using System;
using Godot;

public partial class ResourceObjectSave : Resource
{
    [Export]
    public bool in_cooldown = false;

    [Export]
    public TimerBar.state last_state = TimerBar.state.NONE;

    [Export]
    public int time_left = 0;

    [Export]
    public int current_durability = 0;

    public ResourceObjectSave() { }

    public ResourceObjectSave(
        bool in_cooldown,
        TimerBar.state last_state,
        int time_left,
        int current_durability
    )
    {
        this.in_cooldown = in_cooldown;
        this.last_state = last_state;
        this.time_left = time_left;
        this.current_durability = current_durability;
    }
}
