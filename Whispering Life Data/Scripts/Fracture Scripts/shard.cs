using Godot;
using System;

public partial class shard : RigidBody2D
{
	private Timer timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Sleeping = true;
		timer = GetNode<Timer>("Timer");
		timer.Connect("timeout", Callable.From(SetSleep));
		timer.Start();
	}

    public override void _Process(double delta)
    {
		//Don't know why, but it keeps Scaling to 1/x of Building x Scale
		this.Scale = new Vector2(1,1);
    }
    private void SetSleep()
	{
		this.Sleeping = true;
	}
}
