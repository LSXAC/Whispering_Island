using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemInfo : Resource
{
    [Export]
    public Array<ItemAttributeBase> attributes;

    [Export]
    public int max_stackable_size = 48;

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

    public bool HasAttribute<T>()
        where T : ItemAttributeBase
    {
        foreach (ItemAttributeBase attribute in attributes)
        {
            if (attribute is T)
                return true;
        }
        return false;
    }

    public T GetAttributeOrNull<T>()
        where T : ItemAttributeBase
    {
        foreach (ItemAttributeBase attribute in attributes)
        {
            if (attribute is T)
                return (T)attribute;
        }
        return null;
    }

    public bool HasAttributByType(Type attributeType)
    {
        foreach (ItemAttributeBase attr in attributes)
        {
            if (attr != null && attr.GetType() == attributeType)
                return true;
        }
        return false;
    }
}
