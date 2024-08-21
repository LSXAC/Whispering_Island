using Godot;
using System;

public partial class action_event_menu : Control
{

	public VBoxContainer vbox;
	
	public override void _Ready()
	{
		vbox = GetNode<CanvasLayer>("CanvasLayer").GetNode<VBoxContainer>("VBoxContainer");
	}

	public void InitMenu(Node2D node)
	{
		if(node != null)
			CreateButton(node);
	}

	public void ClearMenu()
	{
		foreach (Node node in vbox.GetChildren())
			node.QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void CreateButton(Node2D node)
	{
		shard_emitter se = node as shard_emitter;
		Button button = new Button();
		button.Text = "Sprengen";
		button.Connect("pressed",Callable.From(se.Shatter));
		vbox.AddChild(button);
	}
}
