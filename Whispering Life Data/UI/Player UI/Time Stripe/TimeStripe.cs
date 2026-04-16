using System;
using Godot;

public partial class TimeStripe : Panel
{
    [Export]
    public TextureRect left,
        right;

    [Export]
    public ColorRect pointer;

    [Export]
    public GradientTexture1D display_gradient;

    public override void _Ready()
    {
        right.FlipH = true;
    }

    public void SetStripe()
    {
        left.Texture = display_gradient;
        right.Texture = display_gradient;
    }

    public void SetPointer(float value)
    {
        pointer.Position = new Vector2(value * 98, 0);
    }
}
