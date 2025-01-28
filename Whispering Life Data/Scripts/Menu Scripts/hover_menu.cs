using System;
using System.Diagnostics;
using Godot;

public partial class hover_menu : PanelContainer
{
    public static hover_menu INSTANCE = null;

    [Export]
    public Label title_Label,
        title_content;

    [Export]
    public Label description_Label,
        description_content;

    [Export]
    public Label hitpoint_Label,
        hitpoint_content;

    [Export]
    public Label resource_type_Label,
        resource_type_content;

    [Export]
    public Label resource_type_level_Label,
        resource_type_level_content;

    public override void _Ready()
    {
        INSTANCE = this;
        title_Label.Text = "PLACEHOLDER TITLE";
        description_Label.Text = "PLACEHOLDER DESC";
        DisableHoverMenu();
    }

    public static void InitHoverMenu(Node2D node)
    {
        if (node is Building_Node b)
        {
            INSTANCE.title_content.Visible = false;
            INSTANCE.title_Label.Visible = false;
            INSTANCE.description_Label.Visible = false;
            INSTANCE.description_content.Visible = false;
            INSTANCE.hitpoint_Label.Visible = false;
            INSTANCE.hitpoint_content.Visible = false;
            INSTANCE.resource_type_Label.Visible = false;
            INSTANCE.resource_type_content.Visible = false;
            INSTANCE.resource_type_level_Label.Visible = false;
            INSTANCE.resource_type_level_content.Visible = false;

            if (b.GetSprite() == null)
                return;

            INSTANCE.title_content.Visible = true;
            INSTANCE.title_Label.Visible = true;
            INSTANCE.description_Label.Visible = true;
            INSTANCE.description_content.Visible = true;
            //Title ---------------
            INSTANCE.title_Label.Text = TranslationServer.Translate("HOVER_MENU_OBJECT") + ":";
            INSTANCE.title_content.Text = TranslationServer.Translate(b.GetTitle());

            //Description -------------
            INSTANCE.description_Label.Text =
                TranslationServer.Translate("HOVER_MENU_DESCRIPTION") + ":";
            INSTANCE.description_content.Text = TranslationServer.Translate(b.GetDescription());
        }

        if (node is ResourceObject ro)
        {
            INSTANCE.hitpoint_Label.Visible = true;
            INSTANCE.hitpoint_content.Visible = true;
            INSTANCE.resource_type_Label.Visible = true;
            INSTANCE.resource_type_content.Visible = true;
            INSTANCE.resource_type_level_Label.Visible = true;
            INSTANCE.resource_type_level_content.Visible = true;

            //Hitpoints if Ressource
            INSTANCE.hitpoint_Label.Text =
                TranslationServer.Translate("HOVER_MENU_HITPOINTS") + ":";
            INSTANCE.hitpoint_content.Text = ro.current_durability + "/" + ro.max_durability;

            //Type if Ressource
            INSTANCE.resource_type_Label.Text =
                TranslationServer.Translate("HOVER_MENU_RESOURCE_TYPE") + ":";
            INSTANCE.resource_type_content.Text = ro.type.ToString();

            //Collect Level if Ressource
            INSTANCE.resource_type_level_Label.Text =
                TranslationServer.Translate("HOVER_MENU_RESOURCE_TYPE_LEVEL") + ":";
            INSTANCE.resource_type_level_content.Text = ro.type_level.ToString();
        }

        EnableHoverMenu();
    }

    public static void DisableHoverMenu()
    {
        INSTANCE.Visible = false;
    }

    public static void EnableHoverMenu()
    {
        INSTANCE.Visible = true;
    }
}
