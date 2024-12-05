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
    public Label times_to_build_left_label;

    [Export]
    public TextureRect window_frame_rect;

    [Export]
    public Item_Row_Manager item_row_manager;

    [Export]
    public Texture2D building_frame;

    [Export]
    public Texture2D removing_frame;

    [Export]
    public PackedScene collected_item_label = ResourceLoader.Load<PackedScene>(
        "res://Prefabs/collect_item_label.tscn"
    );

    [Export]
    public Control collected_item_parent;

    [Export]
    public Panel gameover_panel;
    public static player_ui INSTANCE;

    [Export]
    public CheckBox skip_tutorial_box;

    [Export]
    public VBoxContainer info_vBox;

    [Export]
    public Button mainmenu_button;

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        UpdateGameTimeLabel();
    }

    public override void _Ready()
    {
        INSTANCE = this;
        gameover_panel.Visible = false;
        game_time_panel = GetNode<PanelContainer>("GameTimePanel");
        game_time_label = game_time_panel.GetChild(0).GetNode<Label>("GameTimeLabel");
        quest_complete_panel = GetNode<Panel>("QuestCompletePanel");
        hslider = GetNode<HSlider>("HSlider");
        stamina_label = hslider.GetNode<Label>("Label");

        before_reg.BgColor = new Color(99f / 255f, 176f / 255f, 57f / 255f, 1f);
        when_reg.BgColor = new Color(1f, 0, 0, 1f);
        stamina_label.Text = TranslationServer.Translate("STAMINA_LEFT");

        hslider.AddThemeStyleboxOverride("grabber_area", before_reg);
        mainmenu_button.Pressed += () => ToMainMenu();
    }

    public void ToMainMenu()
    {
        gameover_panel.Visible = false;
        GameMenu.INSTANCE.OnBackToMainMenu();
    }

    public static void CompleteQuestPanelShow()
    {
        INSTANCE.quest_complete_panel.Visible = true;
        INSTANCE.qcp_timer.Start();
    }

    public static void LastQuestPanelShow()
    {
        INSTANCE.quest_complete_panel.Visible = true;
        INSTANCE.quest_complete_panel.GetChild(0).GetChild(0).GetNode<Label>("Label").Text =
            "Last Quest Complete!";
        INSTANCE.quest_complete_panel.GetChild(0).GetChild(0).GetNode<Label>("Label2").Text =
            "Thanks for Playing!";
        //INSTANCE.qcp_timer.Start();
    }

    public void OnCompleteQuestCompletepanel()
    {
        quest_complete_panel.Visible = false;
    }

    public void UpdateGameTimeLabel()
    {
        if (game_time_label == null)
            return;

        game_time_label.Text =
            TranslationServer.Translate("PLAYERUI_GAMETIME")
            + ": "
            + Game_Manager.game_time_since_start.ToString("N2")
            + "s";
    }

    public void OnNewGameButton()
    {
        if (skip_tutorial_box.ButtonPressed)
            Game_Manager.INSTANCE.NewGame(true);
        else
            Game_Manager.INSTANCE.NewGame();
    }

    public void OnLoadGameButton()
    {
        GameMenu.INSTANCE.OnLoadButton();
    }

    public static void AddItemLabelUI(string text)
    {
        Label label = (Label)INSTANCE.collected_item_label.Instantiate();
        label.Text = text;
        INSTANCE.collected_item_parent.AddChild(label);
    }

    public void SetWindowFrame()
    {
        window_frame_rect.Visible = true;
        switch (Game_Manager.building_mode)
        {
            case Game_Manager.BuildingMode.Placing:
                window_frame_rect.Texture = building_frame;
                info_vBox.Visible = true;
                break;
            case Game_Manager.BuildingMode.Removing:
                window_frame_rect.Texture = removing_frame;
                info_vBox.Visible = false;

                break;
            case Game_Manager.BuildingMode.None:
                window_frame_rect.Visible = false;
                break;
        }
    }

    public override void _Process(double delta)
    {
        if (Game_Manager.gameover && !gameover_panel.Visible)
            gameover_panel.Visible = true;

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
