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
    public static QuestMiniPanel instance = null;

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        UpdateTimeLabel(QuestManager.current_quest_time);
    }

    public override void _Ready()
    {
        instance = this;
        UpdateTimeLabel(0);
    }

    public void UpdateTimeLabel(int time)
    {
        int min = time / 60;
        int sec = time - min * 60;
        time_label.Text =
            TranslationServer.Translate("PLAYERUI_QUEST_TIME_LEFT")
            + ": "
            + min.ToString("D2")
            + ":"
            + sec.ToString("D2");
    }

    public override void _Process(double delta)
    {
        if (!GameManager.instance.tutorial_finished && GetChild<Control>(0).Visible)
            GetChild<Control>(0).Visible = false;
        else if (GameManager.instance.tutorial_finished && !GetChild<Control>(0).Visible)
            GetChild<Control>(0).Visible = true;
    }

    public void InitQuestMiniPanel(QuestInfo currentQuest)
    {
        foreach (h_box_item hbi in hbox_items)
            hbi.Visible = false;

        UpdateQuestMiniPanel(currentQuest);
    }

    public void UpdateQuestMiniPanel(QuestInfo currentQuest)
    {
        if (PlayerInventoryUI.instance == null)
        {
            Debug.Print("PlayerInventoryUI.instance is null, cannot update quest mini panel.");
            return;
        }
        Array<Item> items_in_inventory = PlayerInventoryUI.instance.GetListOfItemsInInventory();
        for (int i = 0; i < currentQuest.required_items.Count; i++)
        {
            Item item = currentQuest.required_items[i];

            Array<Item> iii = PlayerInventoryUI.instance.GetItemFromListOrNull(
                items_in_inventory,
                item
            );

            // Clone to keep the original unmodified
            Item item_ref = item.Clone();
            item_ref.amount = (int)(item_ref.amount * GameManager.difficulty_multiplier);

            hbox_items[i].Visible = true;

            if (QuestManager.next_quest_is_doubled_items)
            {
                Item item2 = item.Clone();
                item2.amount = item_ref.amount * 2;
                hbox_items[i].InitItemUI(item2);
            }
            else
                hbox_items[i].InitItemUI(item_ref);

            hbox_items[i].ChangeColor(h_box_item.colorType.white);

            if (iii == null)
            {
                Debug.Print("Quest mini Panel: Item not in Inventory: " + item.info.name);
                if (QuestManager.next_quest_is_doubled_items)
                    hbox_items[i].item_label.Text = "0x / " + (item_ref.amount * 2) + "x";
                else
                    hbox_items[i].item_label.Text = "0x / " + item_ref.amount + "x";
                continue;
            }
            int amount = 0;
            if (iii != null)
                foreach (Item i_x in iii)
                    amount += i_x.amount;

            hbox_items[i].item_label.Text = amount + "x /" + hbox_items[i].item_label.Text;
            if (!QuestManager.next_quest_is_doubled_items)
            {
                if (
                    amount
                    >= (int)(
                        currentQuest.required_items[i].amount * GameManager.difficulty_multiplier
                    )
                )
                    hbox_items[i].ChangeColor(h_box_item.colorType.green);
            }
            else
            {
                if (
                    amount
                    >= (int)(
                        currentQuest.required_items[i].amount * GameManager.difficulty_multiplier
                    ) * 2
                )
                    hbox_items[i].ChangeColor(h_box_item.colorType.green);
            }
        }
    }
}
