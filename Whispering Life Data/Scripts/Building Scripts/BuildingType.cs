using Godot;

[GlobalClass]
public partial class BuildingType : Resource
{
    [Export]
    public PackedScene building_scene;

    [Export]
    public Recipe building_recipe;

    [Export]
    public CATEGORY category;

    public enum CATEGORY
    {
        PRODUCTION,
        DECORATION,
        RESEARCH
    }
}
