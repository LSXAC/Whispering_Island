using System;
using System.Collections;
using System.Diagnostics;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemInfo : Resource
{
    [Export]
    public Array<ItemType> item_types_arr;

    public enum Type
    {
        RESOURCE,
        TOOL,
        WEAPON,
        CLOTHS,
        PLACEABLE,
        ALL,
        SMELTABLE,
        BURNABLE,
        PROCESSED,
        RESEARCHABLE,
        HEAD,
        CHESTPLATE,
        LEGGINGS,
        SHOES
    };

    public enum Type_Level
    {
        Hand,
        Wood,
        Stone,
        Mystic,
        Iron,
    }

    [Export]
    public int max_slot_amount = 48;

    [Export]
    public string item_name;

    [Export]
    public InventoryBase.ITEM_ID unique_id;

    [Export]
    public string item_description;

    [Export]
    public Texture2D texture;

    [Export(PropertyHint.Range, "0,1000,1")]
    public float value;

    [Export]
    public Type_Level type_level;

    [Export]
    public StatsPanel.stat_types use_type;

    [Export]
    public Array<ItemStats> item_stats = new Array<ItemStats>();

    public bool HasType(Type type)
    {
        foreach (ItemType item_type in item_types_arr)
            if (item_type.type == type)
                return true;
        return false;
    }

    public int GetTypeIndex(Type type)
    {
        for (int i = 0; i < item_types_arr.Count; i++)
            if (item_types_arr[i].type == type)
                return i;

        Debug.Print("Item cannot be found in Arr");
        return -1;
    }
}
