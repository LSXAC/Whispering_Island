using System;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;

public partial class Skilltree : ColorRect
{
    [Export]
    public Array<SkillBox> skillboxes;

    [Export]
    public Label research_points_label;
    public static int[] skill_progress = new int[4] { -1, -1, -1, -1 };

    public enum SKILLTYPE
    {
        MOVEMENT,
        FATIGUE,
        HIT,
        RESEARCH_TIME
    }

    public static float[,] bonis =
    {
        { 0.05f, 0.1f, 0.15f },
        { -0.05f, -0.1f, -0.2f },
        { 0.2f, 0.4f, 0.6f },
        { -0.1f, -0.3f, -0.5f }
    };

    public void OnVisiblityChanged()
    {
        research_points_label.Text =
            TranslationServer.Translate("SKILLTREE_RESEARCH_POINTS")
            + ": "
            + ResearchTab.INSTANCE.Research_Points;

        foreach (SkillBox box in skillboxes)
            box.InitSkillBox();
    }
}
