using System;
using System.Diagnostics;
using Godot;

public partial class hover_menu : PanelContainer
{
    public static hover_menu INSTANCE = null;

    [Export]
    public Label title_Label;

    [Export]
    public Label description_Label;

    public override void _Ready()
    {
        INSTANCE = this;
        title_Label.Text = "PLACEHOLDER TITLE";
        description_Label.Text = "PLACEHOLDER DESC";
        DisableHoverMenu();
    }

    public static void InitHoverMenu(Building_Node node)
    {
        if (node.GetSprite() == null)
            return;
        INSTANCE.title_Label.Text = TranslationServer.Translate(node.GetTitle());
        INSTANCE.description_Label.Text = TranslationServer.Translate(node.GetDescription());
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
