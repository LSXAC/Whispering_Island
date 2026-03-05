using System;
using System.Diagnostics;
using DialogueManagerRuntime;
using Godot;
using Godot.Collections;

public partial class QuestMenu : CanvasLayer
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
    public Label reward_label;
    public PackedScene h_box_item = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://bnf8yngk7oyy0")
    );
    public static QuestMenu instance = null;
    public static QuestInfo currentQuest = null;

    private Array<Label> item_labels = new Array<Label>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        instance = this;
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
        {
            if (GameMenu.IsThisWindow(this))
                OnCloseButton();
        }
    }

    public void OnOpenQuestMenu()
    {
        GameMenu.SetWindow(this);
    }

    public void OnCloseButton()
    {
        CutsceneManager.In_Cutscene = true;
        GlobalFunctions.MoveCameraToPosition(new Vector2(13, -256));
        if (TranslationServer.GetLocale() == "de")
            DialogueManager.ShowExampleDialogueBalloon(dialogue_timeline, "Quest_Menu_Closed_DE");
        else
            DialogueManager.ShowExampleDialogueBalloon(dialogue_timeline, "Quest_Menu_Closed_ENG");
        CloseQuestMenu();
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
        int multi = 1;
        if (QuestManager.next_quest_is_doubled_items)
            multi = 2;
        CreateLabels(
            QuestManager.instance.quests[QuestManager.current_quest_id].required_items,
            quest_label_parent,
            multi
        );

        if (
            GlobalFunctions.HasItemsInInventory(
                QuestManager.instance.quests[QuestManager.current_quest_id].required_items
            )
        )
            complete_button.Disabled = false;
        else
            complete_button.Disabled = true;
    }

    public static void CreateLabels(Array<Item> items, Control parent, int multi = 1)
    {
        if (PlayerInventoryUI.instance == null)
        {
            GD.PrintErr("PlayerInventoryUI.instance is null in QuestMenu.CreateLabels");
            return;
        }
        Array<Item> items_in_inventory = PlayerInventoryUI.instance.GetListOfItemsInInventory();

        foreach (Item item in items)
        {
            h_box_item c_label = (h_box_item)instance.h_box_item.Instantiate();
            Array<Item> iii = PlayerInventoryUI.instance.GetItemFromListOrNull(
                items_in_inventory,
                item
            );

            Item item_ref = item.Clone();

            parent.AddChild(c_label);
            item_ref.amount = (int)(item_ref.amount * GameManager.difficulty_multiplier) * multi;
            c_label.InitItemUI(item_ref);

            c_label.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            c_label.AddThemeConstantOverride("separation", 2);
            if (iii == null)
            {
                c_label.item_label.Text = "0x /" + item_ref.amount + "x";
                continue;
            }

            int amount = 0;
            if (iii != null)
                foreach (Item i_x in iii)
                    amount += i_x.amount;

            c_label.item_label.Text = amount + "x /" + item_ref.amount + "x";

            if (amount >= item_ref.amount)
                c_label.ChangeColor(global::h_box_item.colorType.green);
            else
                c_label.ChangeColor(global::h_box_item.colorType.white);
        }
    }
}
