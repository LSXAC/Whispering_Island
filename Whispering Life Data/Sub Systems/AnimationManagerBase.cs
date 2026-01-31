using System;
using Godot;

public partial class AnimationManagerBase : Node2D
{
    [Export]
    public AnimatedSprite2D anim_sprite;
    public string dir = "";

    public void setFrame(int frame, double diff)
    {
        // Prüfe, ob das Objekt noch im Szenenbaum ist
        SceneTree tree = null;

        if (IsInsideTree())
            tree = GetTree();
        else
            return;

        if (!IsInstanceValid(this) || tree == null)
            return;

        var timer = tree.CreateTimer(diff);
        timer.Timeout += () =>
        {
            if (!IsInstanceValid(this))
                return;

            if (!IsInsideTree())
                return;

            SetAnim(frame);
        };
    }

    private void SetAnim(int frame)
    {
        anim_sprite.Play(dir);
        anim_sprite.Frame = frame;
    }
}
