using System;
using Godot;

public partial class SkilltreeBox : Panel
{
    [Export]
    public Label movement_label,
        fatigue_label,
        hit_label,
        research_time;

    public static SkilltreeBox INSTANCE;

    public override void _Ready()
    {
        INSTANCE = this;
        InitMove(
            Skilltree.SKILLTYPE.MOVEMENT,
            Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.MOVEMENT)
        );
        InitMove(
            Skilltree.SKILLTYPE.FATIGUE,
            Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.FATIGUE)
        );
        InitMove(Skilltree.SKILLTYPE.HIT, Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT));
        InitMove(
            Skilltree.SKILLTYPE.RESEARCH_TIME,
            Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.RESEARCH_TIME)
        );
    }

    public void InitMove(Skilltree.SKILLTYPE type, float percent)
    {
        switch (type)
        {
            case Skilltree.SKILLTYPE.MOVEMENT:
                movement_label.Text = type.ToString() + ": " + percent.ToString("P");
                break;
            case Skilltree.SKILLTYPE.FATIGUE:
                fatigue_label.Text = type.ToString() + ": " + percent.ToString("P");
                break;
            case Skilltree.SKILLTYPE.HIT:
                hit_label.Text = type.ToString() + ": " + percent.ToString("P");
                break;
            case Skilltree.SKILLTYPE.RESEARCH_TIME:
                research_time.Text = type.ToString() + ": " + percent.ToString("P");
                break;
        }
    }
}
