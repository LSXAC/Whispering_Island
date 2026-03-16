using System;
using System.Diagnostics;
using DialogueManagerRuntime;
using Godot;
using Godot.Collections;

public partial class QuestMenu : ColorRect
{
    [Export]
    public Control quest_label_parent;

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
    public PackedScene h_box_item = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://bnf8yngk7oyy0")
    );
    public static QuestInfo currentQuest = null;

    private Array<Label> item_labels = new Array<Label>();

    public override void _Ready()
    {
        quest_inventory.OnItemChanged += CheckQuest;
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
        QuestManager.instance.NextQuest();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("Escape"))
            GameMenu.instance.OnExitButton();
    }

    public void OnOpenQuestMenu()
    {
        GameMenu.instance.OnOpenQuestTab();
        OnVisiblityChanged();
    }

    public void CloseQuestMenu()
    {
        GameMenu.CloseLastWindow();
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
        quest_name_label.Text =
            "[center]"
            + TranslationServer.Translate(
                QuestManager.instance.quests[QuestManager.current_quest_id].quest_name
            );
        quest_description_label.Text =
            "[center]"
            + TranslationServer.Translate(
                QuestManager.instance.quests[QuestManager.current_quest_id].quest_description
            );
        reward_label.Text = QuestManager
            .instance.quests[QuestManager.current_quest_id]
            .reward_money.ToString();

        CheckQuest();
    }

    public void CheckQuest()
    {
        ResetParent();
        int multi = 1;
        if (QuestManager.next_quest_is_doubled_items)
            multi = 2;

        CreateLabels(
            QuestManager.instance.quests[QuestManager.current_quest_id].required_items,
            quest_label_parent,
            multi
        );
        if (
            InventoryContainsQuestItems(
                quest_inventory.inventory_items,
                QuestManager.instance.quests[QuestManager.current_quest_id].required_items,
                multi
            )
        )
            complete_button.Disabled = false;
        else
            complete_button.Disabled = true;
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

            foreach (ItemSave invItem in inventory)
            {
                if (invItem == null)
                    continue;

                if (invItem.item_id == (int)questItem.info.id)
                    amount += invItem.amount;
            }

            if (amount < questItem.amount * multi)
                return false;
        }

        return true;
    }

    public static void CreateLabels(Array<Item> questItems, Control parent, int multi = 1)
    {
        foreach (Item item in questItems)
        {
            h_box_item label = (h_box_item)GameMenu.questMenu.h_box_item.Instantiate();
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
}
