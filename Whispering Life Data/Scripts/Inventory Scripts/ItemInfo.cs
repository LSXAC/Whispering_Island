using System;
using System.Collections;
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
        RESEARCHABLE
    };

    [Export]
    public int unique_item_id = -1;

    [Export]
    public string item_name;

    [Export]
    public string item_description;

    [Export]
    public Texture2D texture;

    [Export(PropertyHint.Range, "0,1000,1")]
    public float value;

    [Export]
    public Array<ItemStats> item_stats = new Array<ItemStats>();

    public bool HasType(Type type)
    {
        foreach (ItemType item_type in item_types_arr)
            if (item_type.type == type)
                return true;
        return false;
    }
}
