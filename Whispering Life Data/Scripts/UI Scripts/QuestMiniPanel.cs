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

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        UpdateTimeLabel(QuestManager.current_quest_time);
    }

    public override void _Ready()
    {
        INSTANCE = this;
        UpdateTimeLabel(0);
    }

    public void UpdateTimeLabel(int time)
    {
        Debug.Print(time.ToString());
        int min = time / 60;
        int sec = (time - min * 60);
        time_label.Text =
            TranslationServer.Translate("PLAYERUI_QUEST_TIME_LEFT")
            + ": "
            + min.ToString("D2")
            + ":"
            + sec.ToString("D2")
            + "s";
    }

    public void InitQuestMiniPanel(Quest currentQuest)
    {
        foreach (h_box_item hbi in hbox_items)
            hbi.Visible = false;

        UpdateQuestMiniPanel(currentQuest);
    }

    public void UpdateQuestMiniPanel(Quest currentQuest)
    {
        Array<Item> items_in_inventory = Inventory.INSTANCE.GetListOfItemsInInventory();
        for (int i = 0; i < currentQuest.quest_items.Count; i++)
        {
            Item item = currentQuest.quest_items[i];

            Array<Item> iii = Inventory.INSTANCE.GetItemFromList(items_in_inventory, item);
            hbox_items[i].Visible = true;

            if (QuestManager.next_quest_is_doubled_items)
                hbox_items[i].InitItemUI("", item.amount * 2, item.item_info.texture);
            else
                hbox_items[i].InitItemUI("", item.amount, item.item_info.texture);

            hbox_items[i].ChangeColor(h_box_item.colorType.white);

            if (iii == null)
            {
                Debug.Print(item.item_info.item_name + "NULL");
                if (QuestManager.next_quest_is_doubled_items)
                    hbox_items[i].item_label.Text = "0x / " + (item.amount * 2) + "x";
                else
                    hbox_items[i].item_label.Text = "0x / " + item.amount + "x";
                continue;
            }
            int amount = 0;
            if (iii != null)
                foreach (Item i_x in iii)
                    amount += i_x.amount;

            hbox_items[i].item_label.Text = amount + "x /" + hbox_items[i].item_label.Text;
            if (!QuestManager.next_quest_is_doubled_items)
            {
                if (amount >= currentQuest.quest_items[i].amount)
                    hbox_items[i].ChangeColor(h_box_item.colorType.green);
            }
            else
            {
                if (amount >= currentQuest.quest_items[i].amount * 2)
                    hbox_items[i].ChangeColor(h_box_item.colorType.green);
            }
        }
    }
}
