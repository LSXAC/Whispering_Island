using System;
using Godot;

public partial class HitLabel : Label
{
    [Export]
    Timer timer;

    public override void _Ready()
    {
        timer.Timeout += () => DestroyLabel();
        Random rnd = new Random();
        Random rnd2 = new Random();

        int t = rnd.Next(-3, 4);
        int t2 = rnd2.Next(-5, 2);

        GetParent<CharacterBody2D>().Velocity = new Vector2(t, -15f + t2);
    }

    public void InitText(string text)
    {
        Text = text;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        GetParent<CharacterBody2D>().MoveAndSlide();
    }

    private void DestroyLabel()
    {
        QueueFree();
    }
}
