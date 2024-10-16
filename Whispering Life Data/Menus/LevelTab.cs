using System;
using Godot;

public partial class LevelTab : PanelContainer
{
    private Label feature_label;

    private Label description_label;

    private Label bonus_label;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        feature_label = GetChild(0).GetChild(0).GetNode<Label>("FeatureLabel");
        description_label = GetChild(0).GetChild(0).GetNode<Label>("DescriptionLabel");
        bonus_label = GetChild(0).GetChild(0).GetNode<Label>("BonusLabel");
        ClearText();
    }

    public void UpdateLevelTab(string feature_name, string feature_desc, string bonus)
    {
        feature_label.Text = feature_name;
        description_label.Text = feature_desc;
        bonus_label.Text = bonus;
    }

    public void ClearText()
    {
        feature_label.Text = "No Researches open";
        description_label.Text = "This Item can not further be researched.";
        bonus_label.Text = "-/-";
    }
}
