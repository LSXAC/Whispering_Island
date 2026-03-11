using System;
using System.Threading;
using Godot;

public partial class PathConnectArea : Area2D
{
    public void DisableArea()
    {
        Monitorable = false;
    }

    public void EnableArea()
    {
        Monitorable = true;
    }
}
