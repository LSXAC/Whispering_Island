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

    public static Dictionary<InventoryBase.ITEM_ID, ResearchLevelManager> researchs =
        new Dictionary<InventoryBase.ITEM_ID, ResearchLevelManager>()
        {
            {
                InventoryBase.ITEM_ID.WOOD,
                ResourceLoader.Load<ResearchLevelManager>(
                    "res://Items/ItemResearchs/Wood_Research_Level_Manager.tres"
                )
            },
            {
                InventoryBase.ITEM_ID.STONE,
                ResourceLoader.Load<ResearchLevelManager>(
                    "res://Items/ItemResearchs/Stone_Research_Level_Manager.tres"
                )
            },
            {
                InventoryBase.ITEM_ID.SAND,
                ResourceLoader.Load<ResearchLevelManager>(
                    "res://Items/ItemResearchs/Sand_Research_Level_Manager.tres"
                )
            },
            {
                InventoryBase.ITEM_ID.MYSTIC_WOOD,
                ResourceLoader.Load<ResearchLevelManager>(
                    "res://Items/ItemResearchs/Mystic_Wood_Research_Level_Manager.tres"
                )
            },
            {
                InventoryBase.ITEM_ID.IRON_ORE,
                ResourceLoader.Load<ResearchLevelManager>(
                    "res://Items/ItemResearchs/Iron_Ore_Research_Level_Manager.tres"
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
        {
            BUILDING_ID.TRASHCAN.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Trashcan.tres")
        },
        {
            BUILDING_ID.CORN_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Planting/Corn_Object.tres")
        },
        {
            BUILDING_ID.CARROT_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Planting/Carrot_Object.tres")
        },
        {
            BUILDING_ID.POTATO_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Planting/Potato_Object.tres")
        },
        {
            BUILDING_ID.WHEAT_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Planting/Wheat_Object.tres")
        },
        {
            BUILDING_ID.TREE_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Planting/Tree_Object.tres")
        },
        {
            BUILDING_ID.MYSTIC_TREE_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Planting/Mystic_Tree_Object.tres")
        },
        {
            BUILDING_ID.MYSTIC_FIBRE_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Planting/Mystic_Fibre_Object.tres")
        },
        {
            BUILDING_ID.STONE_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Stone_Object.tres")
        },
        {
            BUILDING_ID.IRON_ORE_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Ore/Iron_Ore_Object.tres")
        },
        {
            BUILDING_ID.COPPER_ORE_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Ore/Copper_Ore_Object.tres")
        },
        {
            BUILDING_ID.SAND_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Sand_Object.tres")
        },
        {
            BUILDING_ID.SAND_STONE_OBJECT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Sand_Stone_Object.tres")
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
        WOODEN_BED,
        TRASHCAN,
        CORN_OBJECT,
        WHEAT_OBJECT,
        POTATO_OBJECT,
        CARROT_OBJECT,
        MYSTIC_TREE_OBJECT,
        MYSTIC_FIBRE_OBJECT,
        TREE_OBJECT,
        STONE_OBJECT,
        IRON_ORE_OBJECT,
        COPPER_ORE_OBJECT,
        SAND_STONE_OBJECT,
        SAND_OBJECT
    }

    public static BuildingType GetBuildingType(BUILDING_ID building_id)
    {
        if (buildings.ContainsKey(building_id.ToString()))
            return buildings[building_id.ToString()];

        GD.PrintErr("No Building found for: " + building_id.ToString());
        return null;
    }
}
