using System;
using Godot;

public partial class SubResearchPanel : Panel
{
    [Export]
    public Label category_title_label;

    [Export]
    public Button category_button;

    public void InitPanel(ItemSubResearchLevel sub_level, int id)
    {
        category_button.Disabled = false;
        category_title_label.Text = sub_level.category.ToString();
        category_button.Pressed += () => OnSelectButton(id);
    }

    public void DisableButton()
    {
        category_button.Disabled = true;
    }

    public void OnSelectButton(int id)
    {
        ResearchTab.instance.DeselectAllSubPanales();
        SetColor(ResearchTab.instance.select_color);
        ResearchTab.instance.OnSelectSubResearch(id);
    }

    public void SetColor(Color color)
    {
        SelfModulate = color;
    }
}
