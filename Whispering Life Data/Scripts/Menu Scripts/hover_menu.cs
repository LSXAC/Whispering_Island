using Godot;
using System;
using System.Diagnostics;

public partial class hover_menu : PanelContainer
{
	public static hover_menu INSTANCE = null;

	[Export]
	public Label title_Label;
	[Export]
	public Label description_Label;
	public override void _Ready()
	{
		INSTANCE = this;
		title_Label.Text = "Title";
		description_Label.Text = "Description";
		DisableHoverMenu();
	}

	public static void InitHoverMenu(Building_Node node)
	{
		INSTANCE.title_Label.Text = node.GetTitle();
		INSTANCE.description_Label.Text = node.GetDescription();
		INSTANCE.Position = new Vector2(node.GetBuildingPosition().X - (INSTANCE.Size.X/2),node.GetBuildingPosition().Y - INSTANCE.Size.Y);
		EnableHoverMenu();
	}

	public static void DisableHoverMenu()
	{
		INSTANCE.Visible = false;
	}
		public static void EnableHoverMenu()
	{
		INSTANCE.Visible = true;
	}
}
