using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class SkillSlot : ColorRect
{
    [Export]
    public bool is_start = false;

    [Export]
    public SkillData.ID id;

    [Export]
    public Array<SkillData.ID> need_skill_ids,
        next_skill_ids;

    public Button button;

    public Array<Line2D> lines;

    public Color normal_color = new Color(1f, 1f, 1f, 1);
    public Color green_color = new Color(0.2f, 1f, 0.2f, 1);

    public override void _Ready()
    {
        base._Ready();
        button = GetNode<Button>("Button");

        if (!is_start)
            button.Disabled = true;
    }

    public bool IsUnlocked()
    {
        if (need_skill_ids == null)
            return false;

        foreach (SkillData.ID id in need_skill_ids)
        {
            if (Skilltree.instance.skill_progress[(int)id] == 0)
                return false;
            Debug.Print("Skill " + id + " is unlocked.");
        }
        return true;
    }

    public void Unlock()
    {
        button.Disabled = false;
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
