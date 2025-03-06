using System;
using Godot;

public partial class AnimationManagerBase : Node2D
{
    [Export]
    public AnimatedSprite2D anim_sprite;
    public string dir = "";

    public void setFrame(int frame, double diff)
    {
        GetTree().CreateTimer(diff).Timeout += () => SetAnim(frame);
    }

    private void SetAnim(int frame)
    {
        if (!IsInstanceValid(this))
            return;
        anim_sprite.Play(dir);
        anim_sprite.Frame = frame;
    }
}
