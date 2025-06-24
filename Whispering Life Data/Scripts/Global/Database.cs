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

    public static Dictionary<Inventory.ITEM_ID, ItemResearch> researchs = new Dictionary<
        Inventory.ITEM_ID,
        ItemResearch
    >()
    {
        {
            Inventory.ITEM_ID.OAK_WOOD,
            ResourceLoader.Load<ItemResearch>(
                "res://Resource Meta/Items/ItemResearchs/item_research_oak_wood.tres"
            )
        },
        {
            Inventory.ITEM_ID.STONE,
            ResourceLoader.Load<ItemResearch>(
                "res://Resource Meta/Items/ItemResearchs/item_research_stone.tres"
            )
        },
        {
            Inventory.ITEM_ID.SAND,
            ResourceLoader.Load<ItemResearch>(
                "res://Resource Meta/Items/ItemResearchs/item_research_sand.tres"
            )
        },
        {
            Inventory.ITEM_ID.MYSTIC_WOOD,
            ResourceLoader.Load<ItemResearch>(
                "res://Resource Meta/Items/ItemResearchs/item_research_mystic_oak_wood.tres"
            )
        },
        {
            Inventory.ITEM_ID.IRON_ORE,
            ResourceLoader.Load<ItemResearch>(
                "res://Resource Meta/Items/ItemResearchs/item_research_iron_ore.tres"
            )
        }
    };

    // BUILDINGS LINES ------------------------------------------------------------------------------------------------

    public static string building_path = "res://Scenes/World Objects/Buildings/building_";
    public static Dictionary<string, Building_Menu_List_Object> buildings = new Dictionary<
        string,
        Building_Menu_List_Object
    >()
    {
        {
            BUILDING_ID.BELT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt.tscn")
        },
        {
            BUILDING_ID.BELT_TUNNEL.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt_tunnel.tscn")
        },
        {
            BUILDING_ID.CHEST.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "chest.tscn")
        },
        {
            BUILDING_ID.FURNACE.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "furnace.tscn")
        },
        {
            BUILDING_ID.TREE_GROWTHER.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "tree_growther.tscn")
        },
        {
            BUILDING_ID.QUARRY.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "quarry.tscn")
        },
        {
            BUILDING_ID.RESEARCH_TABLE.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "research_table.tscn")
        },
        {
            BUILDING_ID.WOOD_BED.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "wood_bed.tscn")
        },
        {
            BUILDING_ID.TRASHCAN.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "trashcan.tscn")
        },
        {
            BUILDING_ID.CORN_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "corn_object.tscn")
        },
        {
            BUILDING_ID.CARROT_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "carrot_object.tscn")
        },
        {
            BUILDING_ID.POTATO_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "potato_object.tscn")
        },
        {
            BUILDING_ID.WHEAT_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "wheat_object.tscn")
        },
        {
            BUILDING_ID.OAK_TREE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "oak_tree_object.tscn")
        },
        {
            BUILDING_ID.MYSTIC_OAK_TREE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(
                building_path + "mystic_oak_tree_object.tscn"
            )
        },
        {
            BUILDING_ID.MYSTIC_FIBRE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(
                building_path + "mystic_fibre_object.tscn"
            )
        },
        {
            BUILDING_ID.STONE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "stone_object.tscn")
        },
        {
            BUILDING_ID.IRON_ORE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "iron_ore_object.tscn")
        },
        {
            BUILDING_ID.COPPER_ORE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "copper_ore_object.tscn")
        },
        {
            BUILDING_ID.SAND_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "sand_object^.tscn")
        },
        {
            BUILDING_ID.SAND_STONE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "sand_stone_object.tscn")
        },
        {
            BUILDING_ID.BELT_SPLITTER_1x2.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt_splitter_1x2.tscn")
        },
        {
            BUILDING_ID.BELT_SPLITTER_1x3.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt_splitter_1x3.tscn")
        },
        {
            BUILDING_ID.BELT_COMBINER_2x1.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt_combiner_2x1.tscn")
        },
        {
            BUILDING_ID.BELT_COMBINER_3x1.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt_combiner_3x1.tscn")
        },
        {
            BUILDING_ID.CHEST_PUFFER.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "chest_puffer.tscn")
        },
        {
            BUILDING_ID.RAIL.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "rail.tscn")
        },
        {
            BUILDING_ID.RAIL_STATION.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "rail_station.tscn")
        },
        {
            BUILDING_ID.MINECART.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "minecart.tscn")
        },
    };

    public enum BUILDING_ID
    {
        BELT,
        BELT_TUNNEL,
        CHEST,
        FURNACE,
        QUARRY,
        TREE_GROWTHER,
        RESEARCH_TABLE,
        WOOD_BED,
        TRASHCAN,
        CORN_OBJECT,
        WHEAT_OBJECT,
        POTATO_OBJECT,
        CARROT_OBJECT,
        MYSTIC_OAK_TREE_OBJECT,
        MYSTIC_FIBRE_OBJECT,
        OAK_TREE_OBJECT,
        STONE_OBJECT,
        IRON_ORE_OBJECT,
        COPPER_ORE_OBJECT,
        SAND_STONE_OBJECT,
        SAND_OBJECT,
        BELT_SPLITTER_1x2,
        BELT_SPLITTER_1x3,
        BELT_COMBINER_2x1,
        BELT_COMBINER_3x1,
        CHEST_PUFFER,
        RAIL,
        RAIL_STATION,
        MINECART
    }

    public static Building_Menu_List_Object GetBuildingMenuListChildObjectInfo(
        BUILDING_ID building_id
    )
    {
        if (buildings.ContainsKey(building_id.ToString()))
            return buildings[building_id.ToString()];

        GD.PrintErr("No Building found for: " + building_id.ToString());
        return null;
    }
}
