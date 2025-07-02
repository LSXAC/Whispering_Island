using System;
using System.Diagnostics;
using Godot;

public partial class BuildMenuListObject : Control
{
    [Export]
    public Button build_button;

    [Export]
    public TextureRect texture;

    [Export]
    public ItemRowManager item_row_manager;

    private Building_Menu_List_Object building_type;

    public override void _Ready()
    {
        build_button.Pressed += () => OnSelectButton();
    }

    public void InitBuildingMenuChild(Building_Menu_List_Object building_type)
    {
        this.building_type = building_type;
        Building_Node placeable = building_type.scene.Instantiate() as Building_Node;

        if (Logger.NodeIsNotNull(texture))
        {
            texture.TooltipText =
                TranslationServer.Translate(((placeable_building)placeable).GetTitle()) + "\n";
            texture.Texture = ((placeable_building)placeable).GetSprite().Texture;
            texture.TooltipText += TranslationServer.Translate(
                ((placeable_building)placeable).GetDescription()
            );
        }

        if (Logger.NodeIsNotNull(item_row_manager) && Logger.NodeIsNotNull(build_button))
        {
            item_row_manager.SetResourcesOnUI(building_type.required_items);
            if (item_row_manager.CheckEnoughResources(building_type.required_items))
                build_button.Disabled = false;
            else
                build_button.Disabled = true;
        }
    }

    public void OnSelectButton()
    {
        BuildMenu.instance.Visible = false;
        BuildMenu.instance.building_placer.InitBuildingFromBuildingMenu(building_type);
    }
}
