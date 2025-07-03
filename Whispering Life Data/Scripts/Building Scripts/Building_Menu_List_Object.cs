using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Building_Menu_List_Object : Resource
{
    [Export]
    public PackedScene scene;

    [Export]
    public Array<Item> required_items;

    [Export]
    public Array<UnlockRequirement> unlock_requirements;

    [Export]
    public BuildMenu.CATEGORY building_menu_category;

    [Export]
    public Texture2D texture_in_build_menu;

    [Export]
    public bool show_object_in_building_menu_list = true;
}
