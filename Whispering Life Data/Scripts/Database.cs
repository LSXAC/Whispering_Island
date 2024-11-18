using System;
using Godot;
using Godot.Collections;

public partial class Database : Node
{
    public enum UPGRADE_LEVEL
    {
        Level0,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5
    }

    // RESEARCH LINES ------------------------------------------------------------------------------------------------

    public static Dictionary<Inventory.ITEM_ID, ResearchLevelManager> researchs = new Dictionary<
        Inventory.ITEM_ID,
        ResearchLevelManager
    >()
    {
        {
            Inventory.ITEM_ID.WOOD,
            ResourceLoader.Load<ResearchLevelManager>(
                "res://Items/ItemResearchs/Wood_Research_Level_Manager.tres"
            )
        }
    };

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
        {
            BUILDING_ID.WOODEN_BED.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Wooden_Bed.tres")
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
        RESEARCH_TABLE,
        WOODEN_BED
    }

    public static BuildingType GetBuildingType(BUILDING_ID building_id)
    {
        if (buildings.ContainsKey(building_id.ToString()))
            return buildings[building_id.ToString()];

        GD.PrintErr("No Building found for: " + building_id.ToString());
        return null;
    }
}
