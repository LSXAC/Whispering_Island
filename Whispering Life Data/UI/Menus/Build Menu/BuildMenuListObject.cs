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
    public TextureRect magic_power_icon;

    [Export]
    public Label title;

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
        Building_Node building_node = building_type.scene.Instantiate() as Building_Node;

        if (Logger.NodeIsNotNull(title))
            title.Text = TranslationServer.Translate(building_node.GetTitle());

        if (Logger.NodeIsNotNull(texture))
        {
            texture.TooltipText = TranslationServer.Translate(building_node.GetTitle()) + "\n";
            texture.TooltipText += TranslationServer.Translate(building_node.GetDescription());
        }
        if (building_type.show_magic_power_use)
            magic_power_icon.Visible = true;
    }

    public void OnSelectButton()
    {
        BuildMenu.instance.Visible = false;
        BuildMenu.instance.building_placer.InitBuildingFromBuildingMenu(building_type, 1);
    }
}
