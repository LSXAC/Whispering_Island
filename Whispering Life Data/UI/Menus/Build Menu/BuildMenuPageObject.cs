using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class BuildMenuPageObject : ColorRect
{
    [Export]
    public Control parent;

    [Export]
    public BuildMenu.CATEGORY category;

    private PackedScene buildingMenuChild = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://cbe4yue61l823")
    );

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

    private void InitBuildings(Building_Menu_List_Object building_type)
    {
        BuildMenuListObject node = buildingMenuChild.Instantiate() as BuildMenuListObject;
        node.InitBuildingMenuChild(building_type);
        parent.AddChild(node);
    }

    public void OnVisiblityChange()
    {
        SetBuildings();
    }
}
