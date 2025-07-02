using System;
using Godot;
using Godot.Collections;

public partial class Database : Node
{
    public static Database instance;

    public override void _Ready()
    {
        instance = this;
        recipies = LoadAllResourcesRecursive<CraftingRecipe>(recipe_path);
        researchs = LoadResearchsRecursive(research_path);
        buildings = LoadBuildingsRecursive(building_path, building_plants_path);
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
    public static Array<CraftingRecipe> recipies = new Array<CraftingRecipe>();
    public static Dictionary<Inventory.ITEM_ID, ItemResearch> researchs =
        new Dictionary<Inventory.ITEM_ID, ItemResearch>();
    public static string building_path =
        "res://Resource Meta/Building Menu List Object Infos/Buildings/";
    public static string building_plants_path =
        "res://Resource Meta/Building Menu List Object Infos/Planting/";
    public static Dictionary<string, Building_Menu_List_Object> buildings =
        new Dictionary<string, Building_Menu_List_Object>();

    // Generische Methode zum rekursiven Laden aller Ressourcen eines Typs
    public static Array<T> LoadAllResourcesRecursive<[MustBeVariant] T>(string directoryPath)
        where T : Resource
    {
        var result = new Array<T>();
        var dir = DirAccess.Open(directoryPath);
        if (dir == null)
        {
            GD.PrintErr($"Directory not found: {directoryPath}");
            return result;
        }

        dir.ListDirBegin();
        string fileName = dir.GetNext();
        while (fileName != "")
        {
            if (dir.CurrentIsDir())
            {
                if (fileName != "." && fileName != "..")
                {
                    var subResult = LoadAllResourcesRecursive<T>(
                        directoryPath.TrimEnd('/') + "/" + fileName
                    );
                    foreach (var res in subResult)
                        result.Add(res);
                }
            }
            else if (fileName.EndsWith(".tres"))
            {
                string fullPath = directoryPath.TrimEnd('/') + "/" + fileName;
                var resource = ResourceLoader.Load<T>(fullPath);
                if (resource != null)
                    result.Add(resource);
            }
            fileName = dir.GetNext();
        }
        dir.ListDirEnd();
        return result;
    }

    // Lädt alle ItemResearchs rekursiv und ordnet sie nach ITEM_ID zu
    public static Dictionary<Inventory.ITEM_ID, ItemResearch> LoadResearchsRecursive(
        string directoryPath
    )
    {
        var result = new Dictionary<Inventory.ITEM_ID, ItemResearch>();
        var researchList = LoadAllResourcesRecursive<ItemResearch>(directoryPath);
        foreach (var research in researchList)
        {
            // Stelle sicher, dass ItemResearch eine id-Property vom Typ Inventory.ITEM_ID hat!
            if (research != null && research.id is Inventory.ITEM_ID id)
                result[id] = research;
        }
        return result;
    }

    // Lädt alle Building_Menu_List_Object aus beiden Pfaden und ordnet sie nach Dateinamen (ohne Endung) zu
    public static Dictionary<string, Building_Menu_List_Object> LoadBuildingsRecursive(
        string buildingsPath,
        string plantsPath
    )
    {
        var result = new Dictionary<string, Building_Menu_List_Object>();
        var allBuildings = LoadAllResourcesRecursive<Building_Menu_List_Object>(buildingsPath);
        var allPlants = LoadAllResourcesRecursive<Building_Menu_List_Object>(plantsPath);

        foreach (var b in allBuildings)
        {
            if (b != null)
                result[System.IO.Path.GetFileNameWithoutExtension(b.ResourcePath)] = b;
        }
        foreach (var p in allPlants)
        {
            if (p != null)
                result[System.IO.Path.GetFileNameWithoutExtension(p.ResourcePath)] = p;
        }
        return result;
    }

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
        string key = building_id.ToString();
        if (buildings.ContainsKey(key))
            return buildings[key];

        GD.PrintErr("No Building found for: " + key);
        return null;
    }
}
