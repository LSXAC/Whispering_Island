using System;
using System.Diagnostics;
using Godot;

public partial class Detector : Area2D
{
    [Export]
    public bool detecting = false;

    [Signal]
    public delegate void belt_detectedEventHandler();

    public void Detect()
    {
        detecting = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!detecting)
            return;

        foreach (Area2D area in GetOverlappingAreas())
        {
            EmitSignal("belt_detected", area);
            detecting = false;
            break;
        }
    }
}
