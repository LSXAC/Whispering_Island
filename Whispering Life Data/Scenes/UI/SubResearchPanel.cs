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
        category_title_label.Text = sub_level.category.ToString();
        category_button.Pressed += () => ResearchTab.instance.OnSelectSubResearch(id);
    }
}
