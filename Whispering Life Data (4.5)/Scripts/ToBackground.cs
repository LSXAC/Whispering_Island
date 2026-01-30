using System;
using Godot;

public partial class ToBackground : TileMapLayer
{
    public override void _Ready()
    {
        base._Ready();
        ZIndex = -100;
    }
}
