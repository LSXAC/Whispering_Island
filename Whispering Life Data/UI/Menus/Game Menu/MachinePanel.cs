using System;
using Godot;

public abstract partial class MachinePanel : Control
{
    public abstract void SetReference(Node2D node);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
