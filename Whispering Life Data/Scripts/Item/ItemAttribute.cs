using System;
using Godot;

[GlobalClass]
public partial class ItemAttribute : Resource
{
    [Export]
    public TYPE type;

    public enum TYPE
    {
        PLACEABLE,
        BURNABLE,
        SMELTABLE,
        RESEARCHABLE,
        PROCESSED,
        WEARABLE,
        TOOL
    }
}
