using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;

public partial class Skilltree : ColorRect
{
    [Export]
    public Label research_points_label;

    [Export]
    public Label title,
        description,
        required_skill_points;

    [Export]
    public Button unlock_skill_button;

    [Export]
    public Array<SkillData> skill_datas;
    public static Skilltree instance;
    public static int[] skill_progress = new int[99];
    public static SkillSlot current_selected_skill = null;

    public enum SKILLTYPE
    {
        MINING_AMOUNT,
        MINING_BONUS,
        DEEP_VEIN,
        RESEARCH_TIME
    }

    public override void _Ready()
    {
        instance = this;
        skill_progress = new int[skill_datas.Count];
    }

    public void OnVisiblityChanged()
    {
        UpdateResearchPoints();
    }

    public void OnUnlockSkillButton()
    {
        SkillData data = GetSkillData(current_selected_skill.id);
        /*if (ResearchTab.instance.Research_Points < data.required_skill_points)
            return;*/

        //ResearchTab.instance.Research_Points -= data.required_skill_points;
        skill_progress[(int)data.id] = 1;

        current_selected_skill.button.Disabled = true;

        /* if (!current_selected_skill.is_start)
             current_selected_skill.SetLineColor(current_selected_skill.green_color);
     */
    }

    public float GetBonusOfCategory(SkillData.TYPE_CATEGORY category)
    {
        float bonus = 0f;
        foreach (SkillData skill in skill_datas)
        {
            if (skill.type_category == category)
            {
                if (skill_progress[(int)skill.id] == 1)
                    if (!skill.is_big_skill)
                        bonus += skill.skill_amount;
                    else
                        GD.PrintErr("Wrong Function used - use HasBigSkill instead!" + skill.id);
            }
        }
        return bonus;
    }

    // e.g. Vein mine etc
    public bool HasBigSkill(SkillData.TYPE_CATEGORY category)
    {
        foreach (SkillData skill in skill_datas)
        {
            if (skill.type_category == category)
                if (skill.is_big_skill)
                {
                    if (skill_progress[(int)skill.id] == 1)
                        return true;
                }
                else
                {
                    GD.PrintErr("No big Skill - use GetBonusOfCategory insead" + skill.id);
                    return false;
                }
        }
        return false;
    }

    public void UpdateResearchPoints()
    {
        research_points_label.Text =
            TranslationServer.Translate("SKILLTREE_RESEARCH_POINTS")
            + ": "
            + ResearchTab.instance.Research_Points;
    }

    public void UpdateViewPanel()
    {
        if (current_selected_skill == null)
        {
            GD.PrintErr("No skill selected!");
            return;
        }

        SkillData data = GetSkillData(current_selected_skill.id);
        if (data == null)
        {
            GD.PrintErr($"SkillData with ID {current_selected_skill.id} not found!");
            return;
        }

        title.Text = TranslationServer.Translate(data.DisplayName);
        description.Text = TranslationServer.Translate(data.Description);
        required_skill_points.Text = "" + data.required_skill_points;
    }

    public SkillData GetSkillData(SkillData.ID id)
    {
        foreach (var skill in skill_datas)
        {
            if (skill.id == id)
                return skill;
        }
        GD.PrintErr($"SkillData with ID {id} not found!");
        return null;
    }
}
