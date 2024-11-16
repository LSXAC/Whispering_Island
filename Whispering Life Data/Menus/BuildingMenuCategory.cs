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

        foreach (var (name, packed) in Database.buildings)
        {
            if (packed == null)
            {
                Debug.Print(name + " empty");
                return;
            }

            if (packed.category == category) //Check if Requirement is there
            {
                if (packed.requirements != null || packed.requirements.Count > 0)
                    if (!CheckAllRequirements(packed.requirements))
                        continue;
                InitBuildings(packed);
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

    private bool CheckAllRequirements(Array<BuildingRequirement> br)
    {
        foreach (BuildingRequirement temp in br)
        {
            if (!ResearchTab.INSTANCE.research_saves.ContainsKey(temp.item_id))
                return false;
            if (
                ResearchTab.INSTANCE.research_saves[temp.item_id].research_level
                < (int)temp.required_level
            )
                return false;
        }
        return true;
    }
}
