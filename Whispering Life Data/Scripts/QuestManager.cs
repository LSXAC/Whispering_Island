using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class QuestManager : Node
{
    [Export]
    public Array<Quest> quests;
    public static int current_quest_id = 0;
    public static QuestManager INSTANCE = null;
    public static int current_quest_time = 0;

    [Export]
    public Timer quest_timer;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public void StartQuest(QuestSave quest_save = null)
    {
        if (quest_save != null)
        {
            current_quest_id = quest_save.current_quest_id;

            if (quest_save.quest_time_left <= 0)
                current_quest_time = quests[current_quest_id].quest_time;
            else
                current_quest_time = quest_save.quest_time_left;
        }
        else
            current_quest_time = quests[current_quest_id].quest_time;

        CheckQuestDuplications();
        StartTimer();
        QuestMenu.INSTANCE.InitQuest(quests[current_quest_id]);
        QuestMiniPanel.INSTANCE.UpdateTimeLabel(current_quest_time);
        QuestMiniPanel.INSTANCE.InitQuestMiniPanel(quests[current_quest_id]);
    }

    private void StartTimer()
    {
        quest_timer.Start();
        quest_timer.WaitTime = 1;
    }

    public void OnQuestTimerTimeout()
    {
        if (current_quest_time - 1 <= 0)
        {
            Game_Manager.INSTANCE.GameOver();
            quest_timer.Stop();
        }

        current_quest_time -= 1;
        QuestMiniPanel.INSTANCE.UpdateTimeLabel(current_quest_time);
    }

    /// <summary>
    /// Check if duplicated ItemType exists inside one Quest. Multiple Items of same Type would reference to same Itemslot and could create a Bug.
    /// </summary>
    /// <param name="obj"></param>
    private void CheckQuestDuplications()
    {
        for (int x = 0; x < quests.Count; x++)
        {
            for (int i = 0; i < quests[x].quest_items.Count; i++)
                if (i + 1 < quests[x].quest_items.Count)
                    if (
                        quests[x].quest_items[i].item_info == quests[x].quest_items[i + 1].item_info
                    )
                        GD.PrintErr("ITEMS IN QUEST " + x + " are in duplicated use");

            if (0 != quests[x].quest_items.Count - 1)
                if (
                    quests[x].quest_items[0]
                    == quests[x].quest_items[quests[x].quest_items.Count - 1]
                )
                    GD.PrintErr("ITEMS IN QUEST " + x + " are in duplicated use");
        }
    }

    public bool CheckQuestComplete()
    {
        foreach (Item quest_item in quests[current_quest_id].quest_items)
        {
            Slot inventory_item = Inventory.INSTANCE.FindSlotWithItemInInventory(
                quest_item.item_info
            );
            if (inventory_item == null)
                return false;
            if (inventory_item.GetItem().amount < quest_item.amount)
                return false;
        }
        return true;
    }

    public void RemoveQuestItems()
    {
        foreach (Item quest_item in quests[current_quest_id].quest_items)
            Inventory.INSTANCE.AddItem(
                quest_item.item_info,
                -quest_item.amount,
                Inventory.INSTANCE.inventory_items
            );
    }

    public void NextQuest()
    {
        RemoveQuestItems();
        QuestMenu.INSTANCE.CloseQuestMenu();
        if (current_quest_id == quests.Count - 1)
        {
            Debug.Print("Last Quest achieved");
            player_ui.LastQuestPanelShow();
            return;
        }
        player_ui.CompleteQuestPanelShow();

        current_quest_id++;
        StartQuest();
    }
}
