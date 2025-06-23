using System;
using System.Collections;
using System.Diagnostics;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemInfo : Resource
{
    [Export]
    public Array<ItemAttribute> attributes;

    public enum MINING_LEVEL
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
    public string name;

    [Export]
    public Inventory.ITEM_ID id;

    [Export]
    public string description;

    [Export]
    public Texture2D texture;

    [Export(PropertyHint.Range, "0,1000,1")]
    public float value;

    [Export]
    public MINING_LEVEL mining_level;

    [Export]
    public StatsPanel.TYPE statspanel_type;

    [Export]
    public Array<ItemStats> stats = new Array<ItemStats>();

    public bool HasAttribute<T>()
        where T : ItemAttribute
    {
        foreach (ItemAttribute attribute in attributes)
        {
            if (attribute is T)
                return true;
        }
        return false;
    }

    public bool HasAttributByType(Type attributeType)
    {
        foreach (ItemAttribute attr in attributes)
        {
            if (attr != null && attr.GetType() == attributeType)
                return true;
        }
        return false;
    }

    public int GetAttributeIndex<T>()
        where T : ItemAttribute
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] is T)
                return i;
        }

        Debug.Print($"Attribute of type {typeof(T).Name} cannot be found in {name}");
        return -1;
    }
}
