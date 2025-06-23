using System;
using Godot;

public partial class ResourceObjectSave : Resource
{
    [Export]
    public bool in_cooldown = false;

    [Export]
    public Vector2 position = Vector2.Zero;

    [Export]
    public TimerBar.STATE last_state = TimerBar.STATE.NONE;

    [Export]
    public int time_left = 0;

    [Export]
    public int current_durability = 0;

    [Export]
    public Database.BUILDING_ID building_id;

    public ResourceObjectSave() { }

    public ResourceObjectSave(
        bool in_cooldown,
        TimerBar.STATE last_state,
        int time_left,
        int current_durability,
        Vector2 pos,
        Database.BUILDING_ID building_id
    )
    {
        this.in_cooldown = in_cooldown;
        this.last_state = last_state;
        this.time_left = time_left;
        this.current_durability = current_durability;
        this.position = pos;
        this.building_id = building_id;
    }
}
