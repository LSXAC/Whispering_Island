using System;
using Godot;
using Godot.Collections;

public partial class BuildingMenuCategory : ColorRect
{
    // Called when the node enters the scene tree for the first time.
    [Export]
    public Control parent;

    [Export]
    public BuildingType.CATEGORY category;

    private PackedScene buildingMenuChild = ResourceLoader.Load<PackedScene>(
        "res://building_menu_child.tscn"
    );

    public override void _Ready() { }

    public void SetBuildings()
    {
        foreach (Control c in parent.GetChildren())
            c.QueueFree();

        foreach (var (name, packed) in Game_Manager.buildings)
        {
            if (packed.category == category)
                InitBuildings(packed);
        }
    }

    private void InitBuildings(BuildingType building_type)
    {
        BuildingMenuChild node = buildingMenuChild.Instantiate() as BuildingMenuChild;
        node.InitBuildingMenuChild(building_type);
        parent.AddChild(node);
    }

    public void OnVisiblityChange()
    {
        SetBuildings();
    }
}
