using Godot;

public partial class BuildingType : Resource
{
    [Export]
    public PackedScene building_scene;

    [Export]
    public Recipe building_recipe;
}
