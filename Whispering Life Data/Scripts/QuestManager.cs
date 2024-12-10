using System;
using System.Diagnostics;
using DialogueManagerRuntime;
using Godot;
using Godot.Collections;

public partial class QuestManager : Node
{
    [Export]
    public Array<Quest> quests;
    public static int current_quest_id = 0;
    public static QuestManager INSTANCE = null;
    public static int current_quest_time = 0;

    public static bool next_quest_half_time = false;
    public static bool next_quest_is_doubled_items = false;

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

    public void PauseTimer()
    {
        quest_timer.Stop();
    }

    public void StartTimer()
    {
        quest_timer.Start();
        quest_timer.WaitTime = 1;
    }

    public async void OnQuestTimerTimeout()
    {
        if (current_quest_time <= 0)
        {
            //Cutscene // UH; WIEK ANNST DEN DU DID WARGEN; GÄ?
            INSTANCE.quest_timer.Stop();
            QuestMenu.INSTANCE.CloseQuestMenu();
            HeartManager.INSTANCE.RemoveHeart();
            if (Game_Manager.gameover)
                return;

            int penealty = -1;
            Random rnd = new Random();
            penealty = (int)rnd.NextInt64(0, 2);
            if (penealty == 1 && next_quest_is_doubled_items)
            {
                next_quest_is_doubled_items = false;
                penealty = 0;
            }

            var dialogue = GD.Load<Resource>("res://Dialogues/Questing.dialogue");
            GlobalFunctions.MoveCamera(new Vector2(0, -256));
            GlobalFunctions.InDialogue();

            if (penealty == 0)
            {
                if (TranslationServer.GetLocale() == "de")
                    DialogueManager.ShowExampleDialogueBalloon(dialogue, "Quest_Not_Completed_DE");
                else
                    DialogueManager.ShowExampleDialogueBalloon(dialogue, "Quest_Not_Completed_ENG");
            }
            if (penealty == 1)
            {
                if (TranslationServer.GetLocale() == "de")
                    DialogueManager.ShowExampleDialogueBalloon(
                        dialogue,
                        "Quest_Not_Completed_1_DE"
                    );
                else
                    DialogueManager.ShowExampleDialogueBalloon(
                        dialogue,
                        "Quest_Not_Completed_1_ENG"
                    );
            }

            await ToSignal(player_ui.INSTANCE.quest_accept_panel.confirm_button, "pressed");
            GlobalFunctions.LeaveDialogue();
            NextQuest(finished_correctly: false, penealty);
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
            Array<Item> iii = Inventory.INSTANCE.GetItemFromList(
                Inventory.INSTANCE.GetListOfItemsInInventory(),
                quest_item
            );

            if (iii == null)
                return false;

            int amount = 0;
            if (iii != null)
                foreach (Item i_x in iii)
                    amount += i_x.amount;

            if (next_quest_is_doubled_items)
            {
                if (amount < quest_item.amount * 2)
                    return false;
            }
            else if (amount < quest_item.amount)
                return false;
        }
        return true;
    }

    public void RemoveQuestItems()
    {
        if (!next_quest_is_doubled_items)
        {
            foreach (Item quest_item in quests[current_quest_id].quest_items)
                Inventory.INSTANCE.RemoveItem(
                    quest_item.item_info,
                    quest_item.amount,
                    Inventory.INSTANCE.inventory_items
                );
        }
        else
        {
            foreach (Item quest_item in quests[current_quest_id].quest_items)
                Inventory.INSTANCE.RemoveItem(
                    quest_item.item_info,
                    quest_item.amount * 2,
                    Inventory.INSTANCE.inventory_items
                );
        }
    }

    public async void NextQuest(bool finished_correctly = true, int penealty = -1)
    {
        if (finished_correctly)
        {
            QuestMenu.INSTANCE.CloseQuestMenu();
            RemoveQuestItems();
        }

        next_quest_is_doubled_items = false;

        if (penealty == (int)QuestAcceptPanel.PENEALTY.DOUBLE_AMOUNT)
            next_quest_is_doubled_items = true;

        if (current_quest_id == quests.Count - 1)
        {
            Debug.Print("Last Quest achieved");
            player_ui.LastQuestPanelShow();
            await ToSignal(player_ui.INSTANCE.qcp_timer, "timeout");
            player_ui.INSTANCE.quest_complete_panel.Visible = false;
            current_quest_id = -1;
        }
        current_quest_id++;
        StartQuest();

        if (next_quest_half_time)
        {
            current_quest_time = (int)(current_quest_time / 2.0);
            next_quest_half_time = false;
        }

        if (finished_correctly)
        {
            player_ui.CompleteQuestPanelShow();
            Game_Manager.In_Cutscene = true;
            PauseTimer();
            await ToSignal(player_ui.INSTANCE.qcp_timer, "timeout");
            StartTimer();
            QuestMenu.INSTANCE.OnOpenQuestMenu();
        }
        Game_Manager.In_Cutscene = false;
    }
}
