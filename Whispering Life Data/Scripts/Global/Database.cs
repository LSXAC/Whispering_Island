using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Database : Node
{
    [Export]
    public Array<Building_Menu_List_Object> building_menu_list_objects { get; set; }

    [Export]
    public Array<ItemResearch> item_research_list { get; set; }

    [Export]
    public Array<CraftingRecipe> crafting_recipies_list { get; set; }

    [Export]
    public Array<ItemInfo> item_info_list { get; set; }

    public static Database instance;

    public override void _Ready()
    {
        instance = this;
        researchs = GetResearchList(item_research_list);
        buildings = GetBuildingsList(building_menu_list_objects);
        Inventory.ITEM_TYPES = GetItemInfoList(item_info_list);
        CheckLoadedResources();
        DebugListValues();
        CheckForDuplicates();
        Debug.Print("Loaded all Resources!");
    }

    public void CheckLoadedResources()
    {
        foreach (BUILDING_ID id in Enum.GetValues(typeof(BUILDING_ID)))
        {
            if (!buildings.ContainsKey(id))
                GD.PrintErr($"Building missing in list: {id}");
        }

        foreach (Inventory.ITEM_ID id in Enum.GetValues(typeof(Inventory.ITEM_ID)))
        {
            if (!Inventory.ITEM_TYPES.ContainsKey(id))
                GD.PrintErr($"Item missing in list: {id}");
        }
    }

    public void CheckForDuplicates()
    {
        // ItemInfo
        var itemInfoIds = new System.Collections.Generic.HashSet<Inventory.ITEM_ID>();
        foreach (var info in item_info_list)
        {
            if (info != null)
            {
                if (!itemInfoIds.Add(info.id))
                    GD.PrintErr($"Duplicate ItemInfo ID: {info.id}");
            }
        }

        // Building_Menu_List_Object
        var buildingIds = new System.Collections.Generic.HashSet<BUILDING_ID>();
        foreach (var bmlo in building_menu_list_objects)
        {
            if (bmlo != null)
            {
                if (!buildingIds.Add(bmlo.scene_building_id))
                    GD.PrintErr($"Duplicate Building ID: {bmlo.scene_building_id}");
            }
        }
    }

    public void DebugListValues()
    {
        Debug.Print("Buildings Found: " + buildings.Count);
        Debug.Print("Items Found: " + Inventory.ITEM_TYPES.Count);
        Debug.Print("Crafting Recipies Found: " + crafting_recipies_list.Count);
        Debug.Print("Researchs Found: " + researchs.Count);
    }

    public enum UPGRADE_LEVEL
    {
        Level0,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5
    }

    public string recipe_path = "res://Resource Meta/Crafting Recipies/";
    public string research_path = "res://Resource Meta/Items/ItemResearchs/";
    public static string building_path =
        "res://Resource Meta/Building Menu List Object Infos/Buildings/";
    public static string objects_path =
        "res://Resource Meta/Building Menu List Object Infos/Objects/";
    public static string building_plants_path =
        "res://Resource Meta/Building Menu List Object Infos/Planting/";
    public static Dictionary<Inventory.ITEM_ID, ItemResearch> researchs =
        new Dictionary<Inventory.ITEM_ID, ItemResearch>();
    public static Dictionary<BUILDING_ID, Building_Menu_List_Object> buildings =
        new Dictionary<BUILDING_ID, Building_Menu_List_Object>();

    public Dictionary<Inventory.ITEM_ID, ItemInfo> GetItemInfoList(Array<ItemInfo> itemInfoList)
    {
        Dictionary<Inventory.ITEM_ID, ItemInfo> dict =
            new Dictionary<Inventory.ITEM_ID, ItemInfo>();
        foreach (ItemInfo info in itemInfoList)
        {
            if (info != null && info.id != Inventory.ITEM_ID.NULL)
            {
                Inventory.ITEM_ID key = info.id;
                dict[key] = info;
            }
        }
        return dict;
    }

    public Dictionary<Inventory.ITEM_ID, ItemResearch> GetResearchList(
        Array<ItemResearch> itemResearchList
    )
    {
        Dictionary<Inventory.ITEM_ID, ItemResearch> dict =
            new Dictionary<Inventory.ITEM_ID, ItemResearch>();
        foreach (ItemResearch research in itemResearchList)
        {
            if (research != null && research.id != Inventory.ITEM_ID.NULL)
            {
                Inventory.ITEM_ID key = research.id;
                dict[key] = research;
            }
        }
        return dict;
    }

    public Dictionary<BUILDING_ID, Building_Menu_List_Object> GetBuildingsList(
        Array<Building_Menu_List_Object> buildings_list
    )
    {
        Dictionary<BUILDING_ID, Building_Menu_List_Object> dict =
            new Dictionary<BUILDING_ID, Building_Menu_List_Object>();
        foreach (Building_Menu_List_Object bmlo in buildings_list)
        {
            if (bmlo != null && bmlo.scene_building_id != BUILDING_ID.NULL)
            {
                BUILDING_ID key = bmlo.scene_building_id;
                dict[key] = bmlo;
            }
        }
        return dict;
    }

    public static Building_Menu_List_Object GetBuildingMenuListChildObjectInfo(
        BUILDING_ID building_id
    )
    {
        BUILDING_ID key = building_id;
        if (buildings.ContainsKey(key))
            return buildings[key];

        GD.PrintErr("No Building found for: " + key);
        return null;
    }

    public enum BUILDING_ID
    {
        NULL,
        BELT_TUNNEL,
        CHEST,
        FURNACE,
        QUARRY,
        TREE_GROWTHER,
        RESEARCH_TABLE,
        WOOD_BED,
        TRASHCAN,
        CORN,
        WHEAT,
        POTATO,
        CARROT,
        MYSTIC_OAK_TREE,
        MYSTIC_FIBRE,
        OAK_TREE,
        STONE,
        IRON_ORE,
        COPPER_ORE,
        SAND_STONE,
        SAND,
        BELT_SPLITTER_1x2,
        BELT_SPLITTER_1x3,
        BELT_COMBINER_2x1,
        BELT_COMBINER_3x1,
        CHEST_WITH_AUTO_REMOVE,
        RAIL,
        RAIL_STATION,
        MINECART,
        FIBRE,
        BELT,
        RAIL_BRIDGE
    }
}
