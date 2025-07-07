using System;
using Godot;

public partial class TimeStripe : Panel
{
    [Export]
    public TextureRect left,
        right;

    [Export]
    public ColorRect pointer;

    public override void _Ready()
    {
        right.FlipH = true;
    }

    public void SetStripe()
    {
        left.Texture = TimeManager.instance.day_night_manager.dayNightGradient;
        right.Texture = TimeManager.instance.day_night_manager.dayNightGradient;
    }

    public void SetPointer(float value)
    {
        pointer.Position = new Vector2(value * 118, 0);
    }
}
