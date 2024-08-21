/*
/ TAKEN FROM https://www.reddit.com/r/godot/comments/nimkqg/how_to_break_a_2d_sprite_in_a_cool_and_easy_way/
/ FROM GDSCRIPT TO C# BY MYSELF Godot C# 4.2.1
*/
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class shard_emitter : Node2D
{
	[Export]
	private short nbr_of_shards = 5;
	[Export]
	private float threshold = 10.0f;
	[Export]
	private float min_impulse = 50.0f;
	[Export]
	private float max_impulse = 200.0f;
	[Export]
	private short lifetime = 5;
	[Export]
	private bool display_triangles = false;

	private PackedScene SHARD = ResourceLoader.Load<PackedScene>("res://Fracture/shard.tscn");
	private Timer deleteTimer;
	private CpuParticles2D explosionParticles;

	List<Vector2[]> triangles = new List<Vector2[]>();
	List<RigidBody2D> shards = new List<RigidBody2D>();
	//shards

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		explosionParticles = GetNode<CpuParticles2D>("CPUParticles2D");
		deleteTimer = GetNode<Timer>("DeleteTimer");
		deleteTimer.WaitTime = lifetime;
		deleteTimer.Connect("timeout",Callable.From(OnTimeout));
		Setup();
	}

    public override void _Process(double delta)
    {
		if (Input.IsKeyPressed(Key.P))
			Shatter();

		//foreach (RigidBody2D rb2d in shards)
		//	GD.Print(rb2d.Sleeping);
    }

    void OnTimeout()
	{
		GetParent().QueueFree();
	}

	void Setup()
	{
		if (GetParent() is not Sprite2D)
			return;

		List<Vector2> points = new List<Vector2>();
		Sprite2D sprite = (Sprite2D)GetParent();
		Godot.Rect2 rect = sprite.GetRect();
		//Create Outer Frame
		points.Add(rect.Position);
		points.Add(rect.Position + new Vector2(rect.Size.X,0));
		points.Add(rect.Position + new Vector2(0, rect.Size.Y));
		points.Add(rect.End);

		//Add Random Breakpoints
		Random rnd = new Random();
		for(int i=0;i<nbr_of_shards;i++)
		{
			Vector2 p = rect.Position + new Vector2((float)rnd.NextDouble() * rect.Size.X,(float)rnd.NextDouble() * rect.Size.Y);
			if (p.X < rect.Position.X + threshold)
				p.X = rect.Position.X;
			else if (p.X > rect.End.X - threshold)
				p.X = rect.End.X;

			if (p.Y < rect.Position.Y + threshold)
				p.Y = rect.Position.Y;
			else if (p.Y > rect.End.Y - threshold)
				p.Y = rect.End.Y;

			points.Add(p);
		}

		//Calculate Triangles
		int[] delaunay = Geometry2D.TriangulateDelaunay(points.ToArray());
		for (int i = 0; i < delaunay.Length; i+=3)
			triangles.Add(new Vector2[] { points[delaunay[i + 2]], points[delaunay[i + 1]], points[delaunay[i]]});

		//Create RigidBody2D Shards
		Texture2D texture = sprite.Texture;
		foreach(Vector2[] t in triangles)
		{
			Vector2 center = new Vector2((t[0].X + t[1].X + t[2].X) / 3.0f, (t[0].Y + t[1].Y + t[2].Y) / 3.0f);
			
			RigidBody2D shard = (RigidBody2D)SHARD.Instantiate();
			shard.Position = center;
			shard.Hide();
			shards.Add(shard);

			//setup Polygons
			Polygon2D poly = shard.GetNode<Polygon2D>("Polygon2D");
			poly.Texture = texture;
			poly.Polygon = t;
			poly.Position = -center;

            //Shrink Polygon
            Godot.Collections.Array<Vector2[]> shrunk_triangle = Geometry2D.OffsetPolygon(t, -2);
			if (shrunk_triangle.Count() > 0)
				shard.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Polygon = shrunk_triangle[0];
			else
				shard.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Polygon = t;
			shard.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Position = -center;
		}

		QueueRedraw();
		CallDeferred("AddChilds");
	}
	//

	void AddChilds()
	{
		foreach (RigidBody2D rb in shards){
			GetParent().AddChild(rb);
			rb.Sleeping = true;
		}
	}

	public void Shatter()
	{
		Random rnd = new Random();
		Sprite2D spr = (Sprite2D)GetParent();
		spr.SelfModulate = new Color(1f,1f,1f,0f);
		foreach(RigidBody2D rb in shards)
		{
			rb.Sleeping = false;
			// 2PI =>  1.5PI - 2PI - 0-0,5PI
			Vector2 direction = Vector2.Up.Rotated((float)(rnd.NextDouble()*3f)-1.5f);
			float impulse = (float)(rnd.NextDouble() * max_impulse) + min_impulse;
			rb.ApplyCentralImpulse(direction*impulse);
			rb.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Disabled = false;
			rb.Show();
		}
		explosionParticles.Emitting = true;
		deleteTimer.Start();
	}

	public override void _Draw()
	{
		if(display_triangles)
			foreach(Vector2[] tri in triangles)
			{
				DrawLine(tri[0],tri[1],new Color(1f,1f,1f),1);
				DrawLine(tri[1],tri[2],new Color(1f,1f,1f),1);
				DrawLine(tri[2],tri[0],new Color(1f,1f,1f),1);
			}

	}
}
