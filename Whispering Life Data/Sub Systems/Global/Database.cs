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
        // Populate lists by scanning the project resources and classifying by type
        PopulateListsFromResources("res://");

        researchs = GetResearchList(item_research_list);
        buildings = GetBuildingsList(building_menu_list_objects);
        Inventory.ITEM_TYPES = GetItemInfoList(item_info_list);
        CheckLoadedResources();
        DebugListValues();
        CheckForDuplicates();
        Debug.Print("Loaded all Resources!");
    }

    // Recursively scan `basePath` for resources, load them and classify by type.
    public void PopulateListsFromResources(string basePath)
    {
        var itemInfos = new System.Collections.Generic.List<ItemInfo>();
        var itemResearches = new System.Collections.Generic.List<ItemResearch>();
        var buildingsTmp = new System.Collections.Generic.List<Building_Menu_List_Object>();
        var craftingTmp = new System.Collections.Generic.List<CraftingRecipe>();

        ScanDirectoryAndClassify(basePath, itemInfos, itemResearches, buildingsTmp, craftingTmp);

        // Sort lists by their enum id order
        itemInfos.Sort((a, b) => ((int)a.id).CompareTo((int)b.id));
        itemResearches.Sort((a, b) => ((int)a.id).CompareTo((int)b.id));
        buildingsTmp.Sort((a, b) => ((int)a.scene_building_id).CompareTo((int)b.scene_building_id));

        // Fill the exported Godot Arrays in the sorted order
        item_info_list = new Array<ItemInfo>();
        foreach (var i in itemInfos)
            item_info_list.Add(i);

        item_research_list = new Array<ItemResearch>();
        foreach (var r in itemResearches)
            item_research_list.Add(r);

        building_menu_list_objects = new Array<Building_Menu_List_Object>();
        foreach (var b in buildingsTmp)
            building_menu_list_objects.Add(b);

        crafting_recipies_list = new Array<CraftingRecipe>();
        foreach (var c in craftingTmp)
            crafting_recipies_list.Add(c);
    }

    private void ScanDirectoryAndClassify(
        string path,
        System.Collections.Generic.List<ItemInfo> itemInfos,
        System.Collections.Generic.List<ItemResearch> itemResearches,
        System.Collections.Generic.List<Building_Menu_List_Object> buildingsTmp,
        System.Collections.Generic.List<CraftingRecipe> craftingTmp
    )
    {
        DirAccess dir = null;
        try
        {
            dir = DirAccess.Open(path);
        }
        catch (Exception)
        {
            return;
        }

        if (dir == null)
            return;

        dir.ListDirBegin();
        string file = dir.GetNext();
        while (!string.IsNullOrEmpty(file))
        {
            if (dir.CurrentIsDir())
            {
                if (file != "." && file != "..")
                {
                    string sub = path.EndsWith("/") ? path + file : path + "/" + file;
                    ScanDirectoryAndClassify(sub, itemInfos, itemResearches, buildingsTmp, craftingTmp);
                }
            }
            else
            {
                string full = path.EndsWith("/") ? path + file : path + "/" + file;
                // Try to load the resource; ignore failures
                var res = ResourceLoader.Load(full);
                if (res == null)
                {
                    // skip non-resource files
                }
                else if (res is ItemInfo ii)
                {
                    if (ii.id != Inventory.ITEM_ID.NULL)
                        itemInfos.Add(ii);
                }
                else if (res is ItemResearch ir)
                {
                    if (ir.id != Inventory.ITEM_ID.NULL)
                        itemResearches.Add(ir);
                }
                else if (res is Building_Menu_List_Object bmlo)
                {
                    if (bmlo.scene_building_id != BUILDING_ID.NULL)
                        buildingsTmp.Add(bmlo);
                }
                else if (res is CraftingRecipe cr)
                {
                    craftingTmp.Add(cr);
                }
            }

            file = dir.GetNext();
        }

        dir.ListDirEnd();
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
        MUSHROOM,
        WHEAT,
        SUNFLOWER,
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
        RAIL_BRIDGE,
        OUTDOOR_LAMP,
        COAL_ORE,
        SUGER_CANE,
        BIG_STONE,
        MYSTIC_MUSHROOM,
        PALM,
        CACTUS
    }
}
