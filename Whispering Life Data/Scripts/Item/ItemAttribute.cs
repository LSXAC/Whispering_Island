using System;
using Godot;

[GlobalClass]
public partial class ItemAttribute : Resource
{
    [Export]
    public ATTRIBUTE attribute;

    public enum ATTRIBUTE
    {
        PLACEABLE,
        BURNABLE,
        SMELTABLE,
        RESEARCHABLE,
        PROCESSED,
    } 
}
