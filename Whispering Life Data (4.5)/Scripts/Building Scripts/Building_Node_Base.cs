using System;
using Godot;

public abstract partial class Building_Node_Base : Node2D
{
    public bool mouse_inside = false;

    public abstract void OnMouseClick();

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton buttonevent)
            if (buttonevent.Pressed && mouse_inside)
            {
                OnMouseClick();
            }
    }
}
