using System;
using System.Diagnostics;
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

        foreach (var (name, scene) in Database.buildings)
        {
            if (scene == null)
            {
                Debug.Print(name + " empty");
                return;
            }

            if (scene.category == category) //Check if Requirement is there
            {
                if (scene.requirements != null || scene.requirements.Count > 0)
                    if (!GlobalFunctions.CheckResearchRequirements(scene.requirements))
                        continue;
                if (!scene.show_object_in_list)
                    continue;
                InitBuildings(scene);
            }
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
