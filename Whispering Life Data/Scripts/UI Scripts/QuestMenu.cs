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
    public PackedScene h_box_item = ResourceLoader.Load<PackedScene>("res://h_box_item.tscn");
    public static QuestMenu INSTANCE = null;
    public static Quest currentQuest = null;

    private Array<Label> item_labels = new Array<Label>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        INSTANCE = this;
    }

    public void InitQuest(Quest quest)
    {
        ResetParent();

        quest_name_label.Text = TranslationServer.Translate(quest.quest_name);
        quest_description_label.Text = TranslationServer.Translate(quest.quest_description);
        CreateLabels(quest.quest_items);
    }

    public void ResetParent()
    {
        foreach (Control child in quest_label_parent.GetChildren())
            child.QueueFree();
    }

    public void OnCompleteButton()
    {
        QuestManager.INSTANCE.NextQuest();
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
        Game_Manager.In_Cutscene = true;
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
                QuestManager.INSTANCE.quests[QuestManager.current_quest_id].quest_name
            );
        quest_description_label.Text =
            "[center]"
            + TranslationServer.Translate(
                QuestManager.INSTANCE.quests[QuestManager.current_quest_id].quest_description
            );
        CreateLabels(QuestManager.INSTANCE.quests[QuestManager.current_quest_id].quest_items);

        if (QuestManager.INSTANCE.CheckQuestComplete())
            complete_button.Disabled = false;
        else
            complete_button.Disabled = true;
    }

    public void CreateLabels(Array<Item> items)
    {
        Array<Item> items_in_inventory = Inventory.INSTANCE.GetListOfItemsInInventory();
        foreach (Item i in items)
        {
            h_box_item c_label = (h_box_item)h_box_item.Instantiate();
            Array<Item> iii = Inventory.INSTANCE.GetItemFromList(items_in_inventory, i);
            if (QuestManager.next_quest_is_doubled_items)
                c_label.InitItemUI(i.item_info.item_name, i.amount, i.item_info.texture);
            else
                c_label.InitItemUI(i.item_info.item_name, i.amount * 2, i.item_info.texture);

            quest_label_parent.AddChild(c_label);
            c_label.Alignment = BoxContainer.AlignmentMode.Center;
            if (iii == null)
            {
                if (QuestManager.next_quest_is_doubled_items)
                    c_label.item_label.Text =
                        TranslationServer.Translate(i.item_info.item_name)
                        + " - "
                        + "0x /"
                        + (i.amount * 2)
                        + "x";
                else
                    c_label.item_label.Text =
                        TranslationServer.Translate(i.item_info.item_name)
                        + " - "
                        + "0x /"
                        + i.amount
                        + "x";
                continue;
            }

            int amount = 0;
            if (iii != null)
                foreach (Item i_x in iii)
                    amount += i_x.amount;

            if (QuestManager.next_quest_is_doubled_items)
                c_label.item_label.Text =
                    TranslationServer.Translate(i.item_info.item_name)
                    + " - "
                    + amount
                    + "x /"
                    + (i.amount * 2)
                    + "x";
            else
                c_label.item_label.Text =
                    TranslationServer.Translate(i.item_info.item_name)
                    + " - "
                    + amount
                    + "x /"
                    + i.amount
                    + "x";

            if (QuestManager.next_quest_is_doubled_items)
            {
                if (amount >= i.amount * 2)
                    c_label.ChangeColor(global::h_box_item.colorType.green);
                else
                    c_label.ChangeColor(global::h_box_item.colorType.white);
            }
            else
            {
                if (amount >= i.amount)
                    c_label.ChangeColor(global::h_box_item.colorType.green);
                else
                    c_label.ChangeColor(global::h_box_item.colorType.white);
            }
        }
    }
}
