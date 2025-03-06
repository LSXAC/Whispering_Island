using System;
using Godot;

public partial class ConnectedBeltsManager : Node2D
{
    public belt_temp[] connected_belts = new belt_temp[4]
    {
        new belt_temp(),
        new belt_temp(),
        new belt_temp(),
        new belt_temp(),
    };

    public enum DIR
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    };

    public bool HasConnectionTo(DIR dir)
    {
        if (connected_belts[(int)dir].connected)
            return true;
        return false;
    }

    public bool HasImportantDirection(DIR from, Belt.Direction dir)
    {
        if (HasConnectionTo(from))
            if (connected_belts[(int)from].to_direction == dir)
                return true;
        return false;
    }
}

public class belt_temp
{
    public bool connected = false;
    public Belt.Direction from_direction,
        to_direction;

    public belt_temp()
    {
        connected = false;
        from_direction = Belt.Direction.NONE;
    }
}
