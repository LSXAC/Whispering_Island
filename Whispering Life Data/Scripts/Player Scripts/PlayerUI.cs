using System;
using Godot;
using Godot.Collections;

public partial class PlayerUI : CanvasLayer
{
    private HSlider hslider;
    private StyleBoxFlat before_reg = new StyleBoxFlat();
    private StyleBoxFlat when_reg = new StyleBoxFlat();
    private Label stamina_label;
    public Panel quest_complete_panel;
    public QuestAcceptPanel quest_accept_panel;

    [Export]
    public EquipmentSelectBar equipmentSelectBar;

    [Export]
    public TimeStripe time_stripe;

    [Export]
    public Label game_time_label;

    [Export]
    public Timer qcp_timer;

    [Export]
    public Label times_to_build_left_label;

    [Export]
    public TextureRect window_frame_rect;

    [Export]
    public ItemRowManager item_row_manager;

    [Export]
    public Texture2D building_frame;

    [Export]
    public Texture2D removing_frame;

    [Export]
    public PackedScene collected_item_label = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/collect_item_label.tscn"
    );

    [Export]
    public Control collected_item_parent;

    [Export]
    public Panel gameover_panel;
    public static PlayerUI instance;

    [Export]
    public Label money_label;

    [Export]
    public VBoxContainer info_vBox;

    [Export]
    public Button mainmenu_button;

    [Export]
    public Timer item_label_timer;

    private Array<string> item_label_queue = new Array<string>();
    private bool queue_working = false;

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        UpdateGameTimeLabel();
    }

    public override void _Ready()
    {
        instance = this;
        gameover_panel.Visible = false;
        quest_complete_panel = GetNode<Panel>("QuestCompletePanel");
        quest_accept_panel = GetNode<QuestAcceptPanel>("QuestAcceptPanel");
        hslider = GetNode<HSlider>("HSlider");
        stamina_label = hslider.GetNode<Label>("Label");

        before_reg.BgColor = new Color(99f / 255f, 176f / 255f, 57f / 255f, 1f);
        when_reg.BgColor = new Color(1f, 0, 0, 1f);
        stamina_label.Text = TranslationServer.Translate("STAMINA_LEFT");

        hslider.AddThemeStyleboxOverride("grabber_area", before_reg);
        mainmenu_button.Pressed += () => ToMainMenu();
        item_label_timer.Timeout += () => SpawnItemLabelUI();
        UpdateMoneyLabel();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (item_label_queue.Count <= 0)
            return;

        if (!queue_working)
        {
            queue_working = true;
            item_label_timer.Start();
        }
    }

    public void SetBuildingUI(Array<Item> required_items)
    {
        if (Logger.NodeIsNotNull(item_row_manager))
            item_row_manager?.SetResourcesOnUI(required_items);
        SetWindowFrame();
    }

    public void ToMainMenu()
    {
        gameover_panel.Visible = false;
        GameMenu.instance.OnBackToMainMenu();
    }

    public static void CompleteQuestPanelShow()
    {
        instance.quest_complete_panel.Visible = true;
        instance.qcp_timer.Start();
    }

    public static void LastQuestPanelShow()
    {
        instance.quest_complete_panel.Visible = true;
        instance.quest_complete_panel.GetChild(0).GetChild(0).GetNode<Label>("Label").Text =
            "Last Quest Complete!";
        instance.quest_complete_panel.GetChild(0).GetChild(0).GetNode<Label>("Label2").Text =
            "Thanks for Playing!";
        instance.qcp_timer.Start();
    }

    public void OnCompleteQuestCompletepanel()
    {
        quest_complete_panel.Visible = false;
    }

    public void UpdateGameTimeLabel()
    {
        if (game_time_label == null)
            return;

        if (game_time_label != null)
            game_time_label.Text = TimeManager.instance.GetTimeFormat();
    }

    public void OnLoadGameButton()
    {
        GameMenu.instance.OnLoadButton();
    }

    public void AddMoney(int amount)
    {
        GameManager.money += amount;

        UpdateMoneyLabel();
    }

    public void RemoveMoney(int amount)
    {
        GameManager.money -= amount;

        UpdateMoneyLabel();
    }

    public void UpdateMoneyLabel()
    {
        if (money_label != null)
            money_label.Text = GameManager.money.ToString();
        else
            GD.PrintErr("PlayerUI money_label is null, cannot update money label.");
    }

    public static void AddItemLabelUI(string text)
    {
        instance.item_label_queue.Add(text);
    }

    public static void AddItemLabelMineableUI(Item item)
    {
        instance.item_label_queue.Add(
            item.amount + "x " + TranslationServer.Translate(item.info.name)
        );
    }

    public static void AddItemLabelMineableBonusItemUI(Item item)
    {
        instance.item_label_queue.Add(
            "Bonus: " + item.amount + "x " + TranslationServer.Translate(item.info.name)
        );
    }

    public static void SpawnItemLabelUI()
    {
        Label label = (Label)instance.collected_item_label.Instantiate();
        label.Text = instance.item_label_queue[0];
        instance.collected_item_parent.AddChild(label);
        instance.item_label_queue.RemoveAt(0);
        instance.queue_working = false;
        instance.item_label_timer.Stop();
    }

    public void SetWindowFrame()
    {
        window_frame_rect.Visible = true;
        switch (GameManager.building_mode)
        {
            case GameManager.BuildingMode.Placing:
                window_frame_rect.Texture = building_frame;
                info_vBox.Visible = true;
                break;
            case GameManager.BuildingMode.Removing:
                window_frame_rect.Texture = removing_frame;
                info_vBox.Visible = false;

                break;
            case GameManager.BuildingMode.None:
                window_frame_rect.Visible = false;
                break;
        }
    }

    public override void _Process(double delta)
    {
        if (GameManager.gameover && !gameover_panel.Visible)
            gameover_panel.Visible = true;

        if (hslider == null)
            return;

        if (
            hslider.MaxValue
            != 1f * Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.STAMINA_MAX)
        )
            hslider.MaxValue =
                1f * Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.STAMINA_MAX);
        if (
            PlayerStamina.current_stamina
                < 1f * Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.STAMINA_MAX)
            && !hslider.Visible
        )
            hslider.Visible = true;
        else if (
            PlayerStamina.current_stamina
                >= 1f * Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.STAMINA_MAX)
            && hslider.Visible
        )
            hslider.Visible = false;

        if (PlayerStamina.stamina_is_regenerating && hslider.Visible)
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
            hslider.Value = PlayerStamina.current_stamina;
    }
}
