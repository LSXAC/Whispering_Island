using System;
using System.Diagnostics;
using Godot;

public partial class LevelTab : PanelContainer
{
    private Label feature_label;

    private Label description_label;

    private Label bonus_label;
    public Database.UPGRADE_LEVEL level;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        feature_label = GetChild(0).GetChild(0).GetNode<Label>("FeatureLabel");
        description_label = GetChild(0).GetChild(0).GetNode<Label>("DescriptionLabel");
        bonus_label = GetChild(0).GetChild(0).GetNode<Label>("BonusLabel");
    }

    public void UpdateLevelTab(
        string translation_string,
        int id,
        string bonus,
        Database.UPGRADE_LEVEL level
    )
    {
        Name = "Level" + id;
        Debug.Print("Update Level Tab");
        feature_label.Text = TranslationServer.Translate(translation_string + "_LVL_" + id);
        description_label.Text = TranslationServer.Translate(
            translation_string + "_LVL_" + id + "_DESC"
        );
        this.level = level;
        bonus_label.Text = bonus;
    }

    public void ClearText()
    {
        feature_label.Text = "No Researches open";
        description_label.Text = "This Item can not further be researched.";
        bonus_label.Text = "-/-";
    }
}
