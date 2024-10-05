using System;
using System.Collections;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemInfo : Resource
{
    public enum Type
    {
        Resource,
        Tool,
        Weapon,
        Cloths,
        Placeable,
        ALL,
        SMELTABLE,
        BURNABLE
    };

    [Export]
    public int unique_item_id = -1;

    [Export]
    public Array<Type> item_types;

    [Export]
    public int burntime = 60;

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
        foreach (Type item_type in item_types)
            if (item_type == type)
                return true;
        return false;
    }
}
