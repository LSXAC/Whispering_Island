using System;
using Godot;

public partial class ShadowManager : CanvasGroup
{
    public override void _Process(double delta)
    {
        base._Process(delta);
        //Invert Lightfactor for Shadows, but keep Alpha between 0,05 and 0,2
        float light_factor = 1f - TimeManager.light_factor;
        SelfModulate = new Color(1f, 1f, 1f, Mathf.Clamp(light_factor * 0.25f, 0.1f, 0.25f));
    }
}
