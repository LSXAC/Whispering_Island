using System;
using Godot;

public partial class CraftingCategoryPanel : PanelContainer
{
    [Export]
    public LabelSettings title_normal,
        title_selected;

    public enum CATEGORIES
    {
        GENERAL,
        TOOLS,
        TOOLPARTS,
        ARMOR,
        AGRICULTURE,
        Machineparts
    }

    public void SetButton(CATEGORIES category)
    {
        foreach (Button btn in GetChild(0).GetChildren())
        {
            if (btn.HasNode("HBoxContainer"))
                btn.GetChild(0).GetNode<Label>("Label").LabelSettings = title_normal;
        }
        GetChild(0).GetChild((int)category).GetChild(0).GetNode<Label>("Label").LabelSettings =
            title_selected;
    }
}
