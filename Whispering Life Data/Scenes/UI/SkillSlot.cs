using System;
using Godot;
using Godot.Collections;

public partial class SkillSlot : ColorRect
{
    [Export]
    public bool is_start = false;

    [Export]
    public SkillData.ID id;
    public Button button;

    public Array<Line2D> lines;

    public Color normal_color = new Color(1f, 1f, 1f, 1);
    public Color green_color = new Color(0.2f, 1f, 0.2f, 1);

    public override void _Ready()
    {
        base._Ready();
        button = GetNode<Button>("Button");
        /*if (!is_start)
        {
            foreach (Node child in GetChildren())
            {
                if (child is Line2D line)
                    lines.Add(line);
            }
            SetLineColor(normal_color);
        }*/
    }

    public void OnButtonClicked()
    {
        Skilltree.current_selected_skill = this;
        Skilltree.instance.UpdateViewPanel();
    }

    /*public void SetLineColor(Color color)
    {
        foreach (Line2D line in lines)
        {
            line.DefaultColor = color;
            line.Visible = true;
        }
    }*/
}
