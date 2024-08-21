using Godot;
using System;

public partial class Building_Collider_Manager : Node2D
{
	[Export]
	private PackedScene building_collider = ResourceLoader.Load<PackedScene>("res://Placeable/BuildingCollider.tscn");

	[Export]
	private TileMap collision_tilemap;

	public override void _Ready()
	{
		CreateBuildingCollider();
	}

	public bool AllCollidersOnBuildingLayer()
	{
		if(GetChildren().Count == 0)
			return false;

		foreach(Node2D node in GetChildren()) {
			BuildingCollider bc = node as BuildingCollider;
			if(!bc.on_building_layer)
				return false;
		}
		return true;
	}

	private void CreateBuildingCollider()
	{
		foreach(Vector2I cell in collision_tilemap.GetUsedCells(0))
		{
			Area2D area = (Area2D)building_collider.Instantiate();
			area.Position = new Vector2(cell.X*16+8,cell.Y*16+8);
			AddChild(area);
		}
	}
}
