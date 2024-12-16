using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class BuildingType : Resource
{
    [Export]
    public PackedScene building_scene;

    [Export]
    public Recipe building_recipe;

    [Export]
    public Array<UnlockRequirement> requirements;

    [Export]
    public CATEGORY category;

    public enum CATEGORY
    {
        PRODUCTION,
        DECORATION,
        PLANTING,
        RESEARCH
    }
}
