using System;
using Godot;
using Godot.Collections;

public partial class Database : Node
{
    // RESEARCH LINES ------------------------------------------------------------------------------------------------

    public static Dictionary<string, ResearchLevelManager> researchs = new Dictionary<
        string,
        ResearchLevelManager
    >()
    {
        /*{
            RESEARCH_ID.WOOD.ToString(),
            ResourceLoader.Load<ResearchLevelManager>("res://Buildings/Belt.tres")
        }*/
    };

    public enum RESEARCH_ID
    {
        NONE,
        WOOD,
        STONE,
        COPPER,
        IRON,
        SAND
    }

    // BUILDINGS LINES ------------------------------------------------------------------------------------------------


    public static Dictionary<string, BuildingType> buildings = new Dictionary<
        string,
        BuildingType
    >()
    {
        {
            BUILDING_ID.BELT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Belt.tres")
        },
        {
            BUILDING_ID.BELTTUNNEL.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Belt_Tunnel.tres")
        },
        {
            BUILDING_ID.CHEST.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Chest.tres")
        },
        {
            BUILDING_ID.FURNACE.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Furnace.tres")
        },
        {
            BUILDING_ID.TREE_GROWTHER.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Tree_Growther.tres")
        },
        {
            BUILDING_ID.QUARRY.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Quarry.tres")
        },
        {
            BUILDING_ID.RESEARCH_TABLE.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Research_Table.tres")
        },
    };

    public enum BUILDING_ID
    {
        BELT,
        BELTTUNNEL,
        CHEST,
        FURNACE,
        QUARRY,
        TREE_GROWTHER,
        RESEARCH_TABLE
    }

    public static BuildingType GetBuildingType(BUILDING_ID building_id)
    {
        if (buildings.ContainsKey(building_id.ToString()))
            return buildings[building_id.ToString()];

        GD.PrintErr("No Building found for: " + building_id.ToString());
        return null;
    }
}
