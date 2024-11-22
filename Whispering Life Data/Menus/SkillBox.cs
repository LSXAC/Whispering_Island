using System;
using Godot;
using Godot.Collections;

public partial class SkillBox : ColorRect
{
    public Label skillLabel;
    public Array<CheckBox> lvl_boxes = new Array<CheckBox>();

    public Button btn;

    public Label resultLabel;

    [Export]
    public Skilltree.SKILLTYPE type;

    public override void _Ready()
    {
        HBoxContainer hbc = GetChild(0).GetChild(0).GetChild<HBoxContainer>(0);
        skillLabel = hbc.GetNode<Label>("Label");
        foreach (Node node in hbc.GetChildren())
        {
            if (node is CheckBox)
                lvl_boxes.Add((CheckBox)node);
        }
        resultLabel = hbc.GetNode<Label>("ResultLabel");
        resultLabel.Text = "";
        btn = hbc.GetNode<Button>("Button");
        btn.Pressed += () => OnUpgradeButton();
        InitSkillBox();
    }

    public void InitSkillBox()
    {
        skillLabel.Text = TranslationServer.Translate("SKILLTREE_" + type.ToString()) + ":";
        foreach (CheckBox box in lvl_boxes)
            box.ButtonPressed = false;

        for (int i = 0; i < 3; i++)
        {
            lvl_boxes[i].Text = Skilltree.bonis[(int)type, i].ToString("P");
        }
        for (int i = 0; i < Skilltree.skill_progress[(int)type] + 1; i++)
        {
            lvl_boxes[i].ButtonPressed = true;
            resultLabel.Text = Skilltree.bonis[(int)type, i].ToString("P");
        }
    }

    public void OnUpgradeButton()
    {
        if (Skilltree.skill_progress[(int)type] == 3)
            return;

        if (ResearchTab.INSTANCE.Research_Points < 1)
            return;

        if (Skilltree.skill_progress[(int)type] < 3)
            Skilltree.skill_progress[(int)type]++;
        ResearchTab.INSTANCE.Research_Points -= 1;
        InitSkillBox();
    }
}
