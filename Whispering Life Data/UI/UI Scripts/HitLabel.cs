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

    public void Init(int amount, Control hit_point)
    {
        Random rnd = new Random();
        int time = rnd.Next(-8, 9);
        Text = "-" + amount;
        GlobalPosition = hit_point.GlobalPosition - new Vector2(11, 10);
        Position += new Vector2(time, 0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        GetParent<CharacterBody2D>().MoveAndSlide();
    }

    private void DestroyLabel()
    {
        GetParent().QueueFree();
    }
}
