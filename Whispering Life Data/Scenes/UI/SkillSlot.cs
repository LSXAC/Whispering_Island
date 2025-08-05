using System;
using Godot;

public partial class SkillSlot : ColorRect
{
    public Line2D line;

    public override void _Ready()
    {
        base._Ready();
        line = GetNodeOrNull<Line2D>("Line2D");
    }

    public void OnButtonClicked()
    {
        if (ResearchTab.instance.Research_Points < 1)
            return;

        SetLineColor(new Color(1, 1, 1, 1));
    }

    public void SetLineColor(Color color)
    {
        if (line != null)
        {
            line.DefaultColor = color;
            line.Visible = true;
        }
    }
}
