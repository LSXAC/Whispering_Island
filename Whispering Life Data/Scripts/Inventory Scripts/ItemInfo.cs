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

    public bool HasAttribute(ItemAttribute.TYPE type)
    {
        foreach (ItemAttribute list_item in attributes)
            if (list_item.type == type)
                return true;
        return false;
    }

    public int GetAttributeIndex(ItemAttribute.TYPE type)
    {
        for (int i = 0; i < attributes.Count; i++)
            if (attributes[i].type == type)
                return i;

        Debug.Print("Attribute cannot be found in " + name);
        return -1;
    }
}
