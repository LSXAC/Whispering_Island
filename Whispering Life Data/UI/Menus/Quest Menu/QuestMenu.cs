using System;
using System.Diagnostics;
using System.Linq;
using DialogueManagerRuntime;
using Godot;
using Godot.Collections;

public partial class QuestMenu : ColorRect
{
    [Export]
    public Control quest_label_parent;

    [Export]
    public Label success_label;

    [Export]
    public RichTextLabel quest_name_label;

    [Export]
    public Button complete_button;

    [Export]
    public RichTextLabel quest_description_label;

    [Export]
    public Resource dialogue_timeline;

    [Export]
    public ChestInventory quest_inventory;

    [Export]
    public Label reward_label;

    [Export]
    public Button exit_button;

    [Export]
    public TabContainer quest_options_tab_container;

    [Export]
    public Control quest_selection_container;

    [Export]
    public Control quest_display_container;

    [Export]
    public ColorRect interaction_break_panel;

    public PackedScene h_box_item = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://bnf8yngk7oyy0")
    );
    public static QuestInfo currentQuest = null;
    public static QuestMenu questMenu;

    private Array<Label> item_labels = new Array<Label>();
    private Array<QuestSelect> quest_select_panels = new Array<QuestSelect>();

    public override void _Ready()
    {
        questMenu = this;
        quest_inventory.OnItemChanged += CheckQuest;

        if (quest_options_tab_container != null)
            for (int i = 0; i < quest_options_tab_container.GetChildCount(); i++)
            {
                QuestSelect questSelect = quest_options_tab_container.GetChild(i) as QuestSelect;
                if (questSelect != null)
                    quest_select_panels.Add(questSelect);
            }
        interaction_break_panel.Visible = false;
    }

    public void InitQuestSelection(int questId)
    {
        if (QuestManager.instance == null)
            return;

        Array<QuestInfo> availableQuests = QuestManager.instance.GetRandomQuestsFromPool(
            questId,
            3
        );

        for (int i = 0; i < quest_select_panels.Count; i++)
        {
            if (i < availableQuests.Count)
                quest_select_panels[i].InitQuestOption(availableQuests[i], i);
            else
                quest_select_panels[i].InitQuestOption(null, -1);
        }

        interaction_break_panel.Visible = true;
        GD.Print("Quest Selection initialized with " + availableQuests.Count + " quests");
    }

    public void OnSelectQuestOption(int optionIndex)
    {
        if (optionIndex < 0 || optionIndex >= quest_select_panels.Count)
            return;

        GetTree().Paused = false;
        interaction_break_panel.Visible = false;
        QuestSelect selectedPanel = quest_select_panels[optionIndex];
        QuestInfo selectedQuest = selectedPanel.GetCurrentQuest();
        exit_button.Disabled = false;

        if (selectedQuest == null)
            return;

        QuestManager.current_selected_quest = selectedQuest;
        currentQuest = selectedQuest;

        double difficultyTimeMultiplier = 1.0 / GameManager.difficulty_multiplier;
        QuestManager.current_quest_time =
            Mathf.RoundToInt(selectedQuest.quest_time * difficultyTimeMultiplier / 5.0) * 5;

        InitQuest(selectedQuest);
        QuestMiniPanel.instance.UpdateTimeLabel(QuestManager.current_quest_time);
        QuestMiniPanel.instance.InitQuestMiniPanel(selectedQuest);
        MonsterIsland.instance.InitializeQuestTimers(QuestManager.current_quest_time);

        ShowQuestDisplay();

        GD.Print("Quest selected: " + selectedQuest.quest_name);
    }

    public void InitQuest(QuestInfo quest)
    {
        ResetParent();

        quest_name_label.Text = TranslationServer.Translate(quest.quest_name);
        quest_description_label.Text = TranslationServer.Translate(quest.quest_description);
        int multi = 1;
        if (QuestManager.next_quest_is_doubled_items)
            multi = 2;
        CreateLabels(quest.required_items, quest_label_parent, multi);
    }

    public void ResetParent()
    {
        foreach (Control child in quest_label_parent.GetChildren())
            child.QueueFree();
    }

    public void OnCompleteButton()
    {
        Random rnd = new Random();
        int step = rnd.Next(0, 100);
        if (step <= MonsterIsland.instance.GetNervRate() * 100)
            QuestManager.instance.NextQuest();
        else
        {
            quest_inventory.ClearInventory();
            QuestManager.instance.ApplyPenality(with_poisoning: true);
        }
    }

    public void OnOpenQuestMenu()
    {
        GameMenu.instance.OnOpenQuestTab();

        if (QuestManager.current_selected_quest == null)
            ShowQuestSelection();
        else
            OnVisiblityChanged();
    }

    public void OnVisiblityChanged()
    {
        if (QuestManager.current_quest_id == -1)
            return;

        ResetParent();
        if (QuestManager.instance == null)
        {
            GD.PrintErr("QuestManager.instance is null in QuestMenu.OnVisiblityChanged");
            return;
        }

        if (QuestManager.current_selected_quest == null)
        {
            GD.PrintErr("No quest selected in QuestMenu.OnVisiblityChanged");
            return;
        }

        quest_name_label.Text =
            "[center]"
            + TranslationServer.Translate(QuestManager.current_selected_quest.quest_name);
        quest_description_label.Text =
            "[center]"
            + TranslationServer.Translate(QuestManager.current_selected_quest.quest_description);
        reward_label.Text = QuestManager.current_selected_quest.reward_money.ToString();

        CheckQuest();
    }

    public void CheckQuest()
    {
        if (QuestManager.current_selected_quest == null)
            return;

        ResetParent();
        int multi = 1;
        if (QuestManager.next_quest_is_doubled_items)
            multi = 2;

        CreateLabels(QuestManager.current_selected_quest.required_items, quest_label_parent, multi);
        if (
            InventoryContainsQuestItems(
                quest_inventory.inventory_items,
                QuestManager.current_selected_quest.required_items,
                multi
            )
        )
            complete_button.Disabled = false;
        else
            complete_button.Disabled = true;

        GlobalFunctions.ItemsWithDamage = CalculateDamagedQuestItems(
            quest_inventory.inventory_items,
            QuestManager.current_selected_quest.required_items,
            multi
        );

        success_label.Text = MonsterIsland.instance.GetNervRate().ToString("P0") + "Sucess Rate.";
        Debug.Print("Items damaged found: " + GlobalFunctions.ItemsWithDamage);
    }

    public static int CalculateDamagedQuestItems(
        ItemSave[] inventory,
        Array<Item> questItems,
        int multi = 1
    )
    {
        int itemsWithDamage = 0;
        MonsterIsland.nerv_abuse = 0;

        foreach (Item questItem in questItems)
        {
            int needed = questItem.amount * multi;
            int countedDamaged = 0;
            float abuse = 0;

            foreach (ItemSave invItem in inventory)
            {
                if (invItem == null)
                    continue;

                if (invItem.item_id == (int)questItem.info.id && invItem.state == 1)
                {
                    int toAdd = Math.Min(invItem.amount, needed - countedDamaged);

                    if (toAdd > 0)
                    {
                        abuse += toAdd * 0.05f;
                        countedDamaged += toAdd;
                    }

                    if (countedDamaged >= needed)
                        break;
                }
            }

            itemsWithDamage += countedDamaged;
            MonsterIsland.nerv_abuse += abuse;
        }

        return itemsWithDamage;
    }

    public static bool InventoryContainsQuestItems(
        ItemSave[] inventory,
        Array<Item> questItems,
        int multi = 1
    )
    {
        foreach (Item questItem in questItems)
        {
            int amount = 0;
            int needed = questItem.amount * multi;

            foreach (ItemSave invItem in inventory)
            {
                if (invItem == null)
                    continue;

                if (invItem.item_id == (int)questItem.info.id)
                {
                    amount += invItem.amount;

                    if (amount >= needed)
                        break;
                }
            }

            if (amount < needed)
                return false;
        }

        return true;
    }

    public static void CreateLabels(Array<Item> questItems, Control parent, int multi = 1)
    {
        PackedScene h_box_item = ResourceLoader.Load<PackedScene>(
            ResourceUid.UidToPath("uid://bnf8yngk7oyy0")
        );

        foreach (Item item in questItems)
        {
            h_box_item label = (h_box_item)h_box_item.Instantiate();
            parent.AddChild(label);

            Item item_ref = item.Clone();
            item_ref.amount = (int)(item_ref.amount * GameManager.difficulty_multiplier) * multi;

            label.InitItemUI(item_ref);
            label.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
            label.AddThemeConstantOverride("separation", 2);

            int amount = 0;

            foreach (ItemSave invItem in GameMenu.questMenu.quest_inventory.inventory_items)
            {
                if (invItem == null)
                    continue;

                if (invItem.item_id == (int)item_ref.info.id)
                    amount += invItem.amount;
            }

            label.item_label.Text = amount + "x / " + item_ref.amount + "x";

            label.ChangeColor(
                amount >= item_ref.amount
                    ? global::h_box_item.colorType.green
                    : global::h_box_item.colorType.white
            );
        }
    }

    public void ShowQuestDisplay()
    {
        if (quest_selection_container != null)
            quest_selection_container.Visible = false;

        if (quest_display_container != null)
            quest_display_container.Visible = true;
    }

    public void ShowQuestSelection()
    {
        exit_button.Disabled = true;

        if (quest_display_container != null)
            quest_display_container.Visible = false;

        if (quest_selection_container != null)
            quest_selection_container.Visible = true;

        if (QuestManager.current_quest_id >= 0 && QuestManager.instance != null)
            InitQuestSelection(QuestManager.current_quest_id);
        GetTree().Paused = true;
    }
}
