using System;
using Godot;

public partial class ConnectedBeltsManager : Node2D
{
    public bool[] connected_belts = new bool[4];

    public enum DIR
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    };

    public bool HasConnectionTo(DIR dir)
    {
        if (connected_belts[(int)dir])
            return true;
        return false;
    }
}
