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
    public Building_Menu.CATEGORY category;

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

            if (scene.building_menu_category == category) //Check if Requirement is there
            {
                if (scene.unlock_requirements != null || scene.unlock_requirements.Count > 0)
                    if (!GlobalFunctions.CheckResearchRequirements(scene.unlock_requirements))
                        continue;
                if (!scene.show_object_in_building_menu_list)
                    continue;
                InitBuildings(scene);
            }
        }
    }

    private void InitBuildings(Building_Menu_List_Object_Info building_type)
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
