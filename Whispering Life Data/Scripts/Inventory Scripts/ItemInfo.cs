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
        ALL
    };

    [Export]
    public int unique_item_id = -1;

    [Export]
    public Type item_type;

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
}
