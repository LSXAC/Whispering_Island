using System;
using System.Collections;
using System.Diagnostics;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemResource : Resource
{
    [Export]
    public Array<ItemType> item_types_arr;

    public enum TYPE
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

    public enum TYPE_LEVEL
    {
        Hand,
        Wood,
        Stone,
        Mystic,
        Iron,
    }

    [Export]
    public bool has_durability = false;

    [Export]
    public int max_durability = 100;

    [Export]
    public int max_slot_amount = 48;

    [Export]
    public string item_name;

    [Export]
    public Inventory.ITEM_ID item_id;

    [Export]
    public string item_description;

    [Export]
    public Texture2D texture;

    [Export(PropertyHint.Range, "0,1000,1")]
    public float value;

    [Export]
    public TYPE_LEVEL type_level;

    [Export]
    public StatsPanel.stat_types use_type;

    [Export]
    public Array<ItemStats> item_stats = new Array<ItemStats>();

    public bool HasType(TYPE type)
    {
        foreach (ItemType item_type in item_types_arr)
            if (item_type.type == type)
                return true;
        return false;
    }

    public int GetTypeIndex(TYPE type)
    {
        for (int i = 0; i < item_types_arr.Count; i++)
            if (item_types_arr[i].type == type)
                return i;

        Debug.Print("Item cannot be found in Arr");
        return -1;
    }
}
