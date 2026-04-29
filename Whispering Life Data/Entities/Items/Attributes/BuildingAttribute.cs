using System;
using Godot;

[GlobalClass]
public partial class BuildingAttribute : ItemAttributeBase
{
    [Export]
    public Callable on_place_callable;

    [Export]
    public Building_Menu_List_Object building_menu_list_object;

    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("BUILDING");
    }
}
