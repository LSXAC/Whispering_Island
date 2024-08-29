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
    public Label quest_name_label;

    [Export]
    public Button complete_button;

    [Export]
    public Label quest_description_label;

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

        quest_name_label.Text = quest.quest_name;
        quest_description_label.Text = quest.quest_description;
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

    public void OnCloseButton()
    {
        Global.MoveCamera(new Vector2(0, -256));
        DialogueManager.ShowDialogueBalloon(dialogue_timeline, "Quest_Menu_Closed_DE");
        CloseQuestMenu();
    }

    public void CloseQuestMenu()
    {
        this.Visible = false;
    }

    public void OnVisiblityChanged()
    {
        if (QuestManager.current_quest_id == -1)
            return;

        Debug.Print("Vis Cha");
        ResetParent();
        CreateLabels(QuestManager.INSTANCE.quests[QuestManager.current_quest_id].quest_items);
        if (QuestManager.INSTANCE.CheckQuestComplete())
            complete_button.Disabled = false;
        else
            complete_button.Disabled = true;
    }

    public void CreateLabels(Array<Item> items)
    {
        Array<Item> items_in_inventory = Inventory.GetListOfItemsInInventory();
        foreach (Item i in items)
        {
            h_box_item c_label = (h_box_item)h_box_item.Instantiate();
            Item iii = Inventory.GetItemFromList(items_in_inventory, i);
            c_label.InitItemUI(i.item_info.item_name, i.amount, i.item_info.texture);
            quest_label_parent.AddChild(c_label);
            c_label.Alignment = BoxContainer.AlignmentMode.Center;
            if (iii == null)
            {
                c_label.item_label.Text = "0x /" + i.amount + "x";
                continue;
            }

            c_label.item_label.Text = iii.amount + "x /" + i.amount + "x";

            if (iii.amount >= i.amount)
                c_label.ChangeColor(global::h_box_item.colorType.green);
            else
                c_label.ChangeColor(global::h_box_item.colorType.white);
        }
    }
}
