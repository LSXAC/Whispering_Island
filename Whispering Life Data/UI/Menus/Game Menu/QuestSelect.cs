using System;
using Godot;
using Godot.Collections;

public partial class QuestSelect : MarginContainer
{
    [Export]
    public Label reward_label;

    [Export]
    public Control quest_label_parent;

    [Export]
    public RichTextLabel quest_name_label;

    [Export]
    public Button select_button;

    [Export]
    public RichTextLabel quest_description_label;

    private QuestInfo current_quest = null;
    private int quest_index = -1;

    public override void _Ready()
    {
        select_button.Pressed += OnSelectButtonPressed;
    }

    public void InitQuestOption(QuestInfo quest, int index)
    {
        current_quest = quest;
        quest_index = index;

        if (quest == null)
        {
            ResetDisplay();
            return;
        }

        quest_name_label.Text = "[center]" + TranslationServer.Translate(quest.quest_name);
        quest_description_label.Text =
            "[center]" + TranslationServer.Translate(quest.quest_description);
        reward_label.Text = quest.reward_money.ToString();

        UpdateQuestItems();
        select_button.Disabled = false;
    }

    private void UpdateQuestItems()
    {
        ResetQuestLabels();

        if (current_quest == null || current_quest.required_items == null)
            return;

        int multi = 1;
        if (QuestManager.next_quest_is_doubled_items)
            multi = 2;

        CreateQuestItemLabels(current_quest.required_items, multi);
    }

    private void CreateQuestItemLabels(Array<Item> questItems, int multi = 1)
    {
        var h_box_item = ResourceLoader.Load<PackedScene>(
            ResourceUid.UidToPath("uid://bnf8yngk7oyy0")
        );

        foreach (Item item in questItems)
        {
            h_box_item label = (h_box_item)h_box_item.Instantiate();
            quest_label_parent.AddChild(label);

            Item item_ref = item.Clone();
            item_ref.amount = (int)(item_ref.amount * GameManager.difficulty_multiplier) * multi;

            label.InitItemUI(item_ref);
            label.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
            label.AddThemeConstantOverride("separation", 2);

            label.item_label.Text = "0x / " + item_ref.amount + "x";
            label.ChangeColor(global::h_box_item.colorType.white);
        }
    }

    private void ResetQuestLabels()
    {
        foreach (Control child in quest_label_parent.GetChildren())
            child.QueueFree();
    }

    private void ResetDisplay()
    {
        quest_name_label.Text = "";
        quest_description_label.Text = "";
        reward_label.Text = "0";
        ResetQuestLabels();
        select_button.Disabled = true;
    }

    private void OnSelectButtonPressed()
    {
        if (current_quest == null || quest_index < 0)
            return;

        QuestMenu.questMenu.OnSelectQuestOption(quest_index);
    }

    public QuestInfo GetCurrentQuest()
    {
        return current_quest;
    }

    public int GetQuestIndex()
    {
        return quest_index;
    }
}
