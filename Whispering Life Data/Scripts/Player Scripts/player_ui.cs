using System;
using Godot;

public partial class player_ui : CanvasLayer
{
    private HSlider hslider;
    private StyleBoxFlat before_reg = new StyleBoxFlat();
    private StyleBoxFlat when_reg = new StyleBoxFlat();
    private Label stamina_label;
    private Panel quest_complete_panel;
    private PanelContainer game_time_panel;
    private Label game_time_label;

    [Export]
    public Timer qcp_timer;

    [Export]
    public PackedScene collected_item_label = ResourceLoader.Load<PackedScene>(
        "res://Prefabs/collect_item_label.tscn"
    );

    [Export]
    public Control collected_item_parent;
    public static player_ui INSTANCE;

    public override void _Ready()
    {
        INSTANCE = this;
        game_time_panel = GetNode<PanelContainer>("GameTimePanel");
        game_time_label = game_time_panel.GetNode<Label>("GameTimeLabel");
        quest_complete_panel = GetNode<Panel>("QuestCompletePanel");
        hslider = GetNode<HSlider>("HSlider");
        stamina_label = hslider.GetNode<Label>("Label");

        before_reg.BgColor = new Color(99f / 255f, 176f / 255f, 57f / 255f, 1f);
        when_reg.BgColor = new Color(1f, 0, 0, 1f);
        stamina_label.Text = TranslationServer.Translate("STAMINA_LEFT");

        hslider.AddThemeStyleboxOverride("grabber_area", before_reg);
    }

    public static void CompleteQuestPanelShow()
    {
        INSTANCE.quest_complete_panel.Visible = true;
        INSTANCE.qcp_timer.Start();
    }

    public void OnCompleteQuestCompletepanel()
    {
        quest_complete_panel.Visible = false;
    }

    public void UpdateGameTimeLabel()
    {
        game_time_label.Text =
            "Game Time: " + Game_Manager.game_time_since_start.ToString("N2") + "s";
    }

    public static void AddItemLabelUI(string text)
    {
        Label label = (Label)INSTANCE.collected_item_label.Instantiate();
        label.Text = text;
        INSTANCE.collected_item_parent.AddChild(label);
    }

    public override void _Process(double delta)
    {
        if (hslider == null)
            return;

        if (Player_Stamina.current_stamina < 1f && !hslider.Visible)
            hslider.Visible = true;
        else if (Player_Stamina.current_stamina >= 1f && hslider.Visible)
            hslider.Visible = false;

        if (Player_Stamina.stamina_is_regenerating && hslider.Visible)
        {
            hslider.AddThemeStyleboxOverride("grabber_area", when_reg);
            stamina_label.Text = TranslationServer.Translate("STAMINA_OUT");
        }
        else
        {
            hslider.AddThemeStyleboxOverride("grabber_area", before_reg);
            stamina_label.Text = TranslationServer.Translate("STAMINA_LEFT");
        }

        if (hslider.Visible)
            hslider.Value = Player_Stamina.current_stamina;
    }
}
