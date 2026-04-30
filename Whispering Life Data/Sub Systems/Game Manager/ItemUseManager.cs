using System;
using System.Diagnostics;
using Godot;

public partial class ItemUseManager : Node2D
{
    public static ItemUseManager instance;

    public override void _Ready()
    {
        instance = this;
    }

    public void UseItem(Item item, Node target = null)
    {
        if (item?.info == null)
            return;

        UseAttribute use_attr = item.info.GetAttributeOrNull<UseAttribute>();
        if (use_attr != null && use_attr.HasEffects())
            ApplyUseEffects(use_attr, target);
    }

    public void BuildItem(Item item)
    {
        if (item?.info == null)
            return;

        BuildingAttribute building_attr = item.info.GetAttributeOrNull<BuildingAttribute>();
        if (building_attr != null && building_attr.building_menu_list_object != null)
        {
            GameMenu.CloseLastWindow();
            BuildMenu.instance.building_placer.InitBuildingFromBuildingMenu(
                building_attr.building_menu_list_object,
                item.amount
            );
        }
    }

    private void ApplyUseEffects(UseAttribute use_attr, Node target)
    {
        if (target == null)
            target = PlayerUI.instance;

        Debug.Print("Item used!");
    }
}
