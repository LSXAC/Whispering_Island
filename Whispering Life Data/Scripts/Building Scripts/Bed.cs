using System;
using Godot;

public partial class Bed : placeable_building
{
    public override void OnMouseClick()
    {
        base.OnMouseClick();

        if (!CheckClickDependencies(this))
            return;

        Player.instance.player_stats.RemoveFatigue(seconds: 5);
    }
}
