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
            Inventory.ITEM_ID.MYSTIC_OAK_WOOD,
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

    public static string building_path =
        "res://Resource Meta/Building Menu List Object Infos/Buildings/build_menu_list_object_";
    public static string building_plants_path =
        "res://Resource Meta/Building Menu List Object Infos/Planting/build_menu_list_object_";
    public static Dictionary<string, Building_Menu_List_Object> buildings = new Dictionary<
        string,
        Building_Menu_List_Object
    >()
    {
        {
            BUILDING_ID.BELT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt.tres")
        },
        {
            BUILDING_ID.BELT_TUNNEL.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt_tunnel.tres")
        },
        {
            BUILDING_ID.CHEST.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "chest.tres")
        },
        {
            BUILDING_ID.FURNACE.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "furnace.tres")
        },
        {
            BUILDING_ID.TREE_GROWTHER.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "tree_growther.tres")
        },
        {
            BUILDING_ID.QUARRY.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "quarry.tres")
        },
        {
            BUILDING_ID.RESEARCH_TABLE.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "research_table.tres")
        },
        {
            BUILDING_ID.WOOD_BED.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "wood_bed.tres")
        },
        {
            BUILDING_ID.TRASHCAN.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "trashcan.tres")
        },
        {
            BUILDING_ID.CORN_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_plants_path + "corn.tres")
        },
        {
            BUILDING_ID.CARROT_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_plants_path + "carrot.tres")
        },
        {
            BUILDING_ID.POTATO_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_plants_path + "potato.tres")
        },
        {
            BUILDING_ID.WHEAT_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_plants_path + "wheat.tres")
        },
        {
            BUILDING_ID.OAK_TREE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_plants_path + "oak_tree.tres")
        },
        {
            BUILDING_ID.MYSTIC_OAK_TREE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(
                building_plants_path + "mystic_oak_tree.tres"
            )
        },
        {
            BUILDING_ID.MYSTIC_FIBRE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(
                building_plants_path + "mystic_fibre.tres"
            )
        },
        {
            BUILDING_ID.DUMMY_STONE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(
                "res://Resource Meta/Building Menu List Object Infos/Buildings/build_menu_list_object_dummy.tres"
            )
        },
        {
            BUILDING_ID.DUMMY_IRON_ORE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(
                "res://Resource Meta/Building Menu List Object Infos/Buildings/build_menu_list_object_dummy.tres"
            )
        },
        {
            BUILDING_ID.DUMMY_COPPER_ORE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(
                "res://Resource Meta/Building Menu List Object Infos/Buildings/build_menu_list_object_dummy.tres"
            )
        },
        {
            BUILDING_ID.DUMMY_SAND_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(
                "res://Resource Meta/Building Menu List Object Infos/Buildings/build_menu_list_object_dummy.tres"
            )
        },
        {
            BUILDING_ID.DUMMY_SAND_STONE_OBJECT.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(
                "res://Resource Meta/Building Menu List Object Infos/Buildings/build_menu_list_object_dummy.tres"
            )
        },
        {
            BUILDING_ID.BELT_SPLITTER_1x2.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt_splitter_1x2.tres")
        },
        {
            BUILDING_ID.BELT_SPLITTER_1x3.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt_splitter_1x3.tres")
        },
        {
            BUILDING_ID.BELT_COMBINER_2x1.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt_combiner_2x1.tres")
        },
        {
            BUILDING_ID.BELT_COMBINER_3x1.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "belt_combiner_3x1.tres")
        },
        {
            BUILDING_ID.CHEST_WITH_AUTO_REMOVE.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(
                building_path + "chest_with_auto_remove.tres"
            )
        },
        {
            BUILDING_ID.RAIL.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "rail.tres")
        },
        {
            BUILDING_ID.RAIL_STATION.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "rail_station.tres")
        },
        {
            BUILDING_ID.MINECART.ToString(),
            ResourceLoader.Load<Building_Menu_List_Object>(building_path + "minecart.tres")
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
        DUMMY_STONE_OBJECT,

        DUMMY_IRON_ORE_OBJECT,

        DUMMY_COPPER_ORE_OBJECT,

        DUMMY_SAND_STONE_OBJECT,

        DUMMY_SAND_OBJECT,
        BELT_SPLITTER_1x2,
        BELT_SPLITTER_1x3,
        BELT_COMBINER_2x1,
        BELT_COMBINER_3x1,
        CHEST_WITH_AUTO_REMOVE,
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
