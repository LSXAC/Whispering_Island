using System;
using Godot;

public partial class Global : Node2D
{
    public static float GetDistanceToPlayer(Vector2 nodePosition)
    {
        return Player.instance.GlobalPosition.DistanceTo(nodePosition);
    }
}
