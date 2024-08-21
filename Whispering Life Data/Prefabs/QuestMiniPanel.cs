using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class QuestMiniPanel : PanelContainer
{
    [Export]
    public Array<h_box_item> hbox_items;

    [Export]
    public Label time_label;
    public static QuestMiniPanel INSTANCE = null;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public void UpdateTimeLabel(int time)
    {
        time_label.Text = "Time Left: " + time + "s";
    }

    public void InitQuestMiniPanel(Quest currentQuest)
    {
        foreach (h_box_item hbi in hbox_items)
            hbi.Visible = false;

        UpdateQuestMiniPanel(currentQuest);
    }

    public void UpdateQuestMiniPanel(Quest currentQuest)
    {
        Array<Item> items_in_inventory = Inventory.GetListOfItemsInInventory();
        Debug.Print("Start Updating");
        for (int i = 0; i < currentQuest.quest_items.Count; i++)
        {
            Item item = currentQuest.quest_items[i];
            Item iii = Inventory.GetItemFromList(items_in_inventory, item);
            hbox_items[i].Visible = true;
            hbox_items[i].InitItemUI("", item.amount, item.item_info.texture);

            if (iii == null)
            {
                Debug.Print(item.item_info.item_name + "NULL");
                hbox_items[i].item_label.Text = "0x / " + item.amount + "x";
                continue;
            }

            hbox_items[i].item_label.Text = iii.amount + "x /" + hbox_items[i].item_label.Text;

            if (iii.amount >= currentQuest.quest_items[i].amount)
                hbox_items[i].ChangeColor(h_box_item.colorType.green);
            else
                hbox_items[i].ChangeColor(h_box_item.colorType.white);
        }
    }
}
