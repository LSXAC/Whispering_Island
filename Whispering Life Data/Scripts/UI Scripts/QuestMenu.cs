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
    public PackedScene h_box_item = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/h_box_item_menu_line.tscn"
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
        CreateLabels(quest.required_items);
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
        GameManager.In_Cutscene = true;
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
        CreateLabels(QuestManager.instance.quests[QuestManager.current_quest_id].required_items);

        if (QuestManager.instance.CheckQuestComplete())
            complete_button.Disabled = false;
        else
            complete_button.Disabled = true;
    }

    public void CreateLabels(Array<Item> items)
    {
        if (PlayerInventoryUI.instance == null)
        {
            GD.PrintErr("PlayerInventoryUI.instance is null in QuestMenu.CreateLabels");
            return;
        }
        Array<Item> items_in_inventory = PlayerInventoryUI.instance.GetListOfItemsInInventory();
        foreach (Item item in items)
        {
            h_box_item c_label = (h_box_item)h_box_item.Instantiate();
            Array<Item> iii = PlayerInventoryUI.instance.GetItemFromListOrNull(
                items_in_inventory,
                item
            );
            if (QuestManager.next_quest_is_doubled_items)
                c_label.InitItemUI(item.info.name, item.amount, item.info.texture);
            else
                c_label.InitItemUI(item.info.name, item.amount * 2, item.info.texture);

            quest_label_parent.AddChild(c_label);
            c_label.Alignment = BoxContainer.AlignmentMode.Center;
            if (iii == null)
            {
                if (QuestManager.next_quest_is_doubled_items)
                    c_label.item_label.Text =
                        TranslationServer.Translate(item.info.name)
                        + " - "
                        + "0x /"
                        + (item.amount * 2)
                        + "x";
                else
                    c_label.item_label.Text =
                        TranslationServer.Translate(item.info.name)
                        + " - "
                        + "0x /"
                        + item.amount
                        + "x";
                continue;
            }

            int amount = 0;
            if (iii != null)
                foreach (Item i_x in iii)
                    amount += i_x.amount;

            if (QuestManager.next_quest_is_doubled_items)
                c_label.item_label.Text =
                    TranslationServer.Translate(item.info.name)
                    + " - "
                    + amount
                    + "x /"
                    + (item.amount * 2)
                    + "x";
            else
                c_label.item_label.Text =
                    TranslationServer.Translate(item.info.name)
                    + " - "
                    + amount
                    + "x /"
                    + item.amount
                    + "x";

            if (QuestManager.next_quest_is_doubled_items)
            {
                if (amount >= item.amount * 2)
                    c_label.ChangeColor(global::h_box_item.colorType.green);
                else
                    c_label.ChangeColor(global::h_box_item.colorType.white);
            }
            else
            {
                if (amount >= item.amount)
                    c_label.ChangeColor(global::h_box_item.colorType.green);
                else
                    c_label.ChangeColor(global::h_box_item.colorType.white);
            }
        }
    }
}
