using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Building_Menu_List_Child_Object_Info : Resource
{
    [Export]
    public PackedScene scene;

    [Export]
    public Recipe recipe;

    [Export]
    public Array<UnlockRequirement> unlock_requirements;

    [Export]
    public Building_Menu.CATEGORY building_menu_category;

    [Export]
    public bool show_object_in_building_menu_list = true;
}
