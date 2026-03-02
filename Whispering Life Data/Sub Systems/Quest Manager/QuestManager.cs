using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class QuestManager : Node
{
    [Export]
    public Array<QuestInfo> quests;
    private Resource quest_dialogue = ResourceLoader.Load<Resource>(
        ResourceUid.UidToPath("uid://c1p7gva6jex80")
    );
    public static int current_quest_id = 0;
    public static QuestManager instance = null;
    public static int current_quest_time = 0;

    public static bool next_quest_half_time = false;
    public static bool next_quest_is_doubled_items = false;

    public QuestTimer quest_timer;
    public QuestPenality quest_penality;

    // Monster Island State References
    private MonsterIsland monster_island;
    private MonsterIslandStateManager island_state_manager;

    public override void _Ready()
    {
        instance = this;
        current_quest_time = 0;
        current_quest_id = 0;
        next_quest_half_time = false;
        next_quest_is_doubled_items = false;

        monster_island = MonsterIsland.instance;
        quest_timer = GetNode<QuestTimer>("QuestTimer");
        quest_penality = GetNode<QuestPenality>("QuestPenality");
        if (monster_island != null)
            island_state_manager = monster_island.GetNode<MonsterIslandStateManager>(
                "StateManager"
            );
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
        {
            current_quest_time =
                Mathf.RoundToInt(
                    quests[current_quest_id].quest_time / GameManager.difficulty_multiplier / 5f
                ) * 5;
        }

        CheckQuestDuplications();
        quest_timer.StartTimer();
        QuestMenu.instance.InitQuest(quests[current_quest_id]);
        QuestMiniPanel.instance.UpdateTimeLabel(current_quest_time);
        QuestMiniPanel.instance.InitQuestMiniPanel(quests[current_quest_id]);
        MonsterIsland.instance.InitializeQuestTimers();
    }

    public async void OnQuestTimerTimeout()
    {
        if (current_quest_time <= 0)
        {
            //Cutscene
            GameMenu.CloseLastWindow();
            instance.quest_timer.Stop();
            QuestMenu.instance.CloseQuestMenu();
            //HeartManager.instance.RemoveHeart();
            if (GameManager.gameover)
                return;

            if (CutsceneManager.skip_cutscenes)
            {
                NextQuest(finished_correctly: false, penalty: -1);
                return;
            }

            int penalty = quest_penality.DeterminePenalty();
            quest_penality.PlayPenaltyCutscene(penalty);
            await ToSignal(PlayerUI.instance.quest_accept_panel.confirm_button, "pressed");
            GlobalFunctions.LeaveDialogue();

            NextQuest(finished_correctly: false, penalty);
        }

        current_quest_time -= GameManager.time_multiplier;
        QuestMiniPanel.instance.UpdateTimeLabel(current_quest_time);
    }

    /// Check if duplicated ItemType exists inside one Quest.
    private void CheckQuestDuplications()
    {
        for (int x = 0; x < quests.Count; x++)
        {
            for (int i = 0; i < quests[x].required_items.Count; i++)
                if (i + 1 < quests[x].required_items.Count)
                    if (quests[x].required_items[i].info == quests[x].required_items[i + 1].info)
                        GD.PrintErr("ITEMS IN QUEST " + x + " are in duplicated use");

            if (0 != quests[x].required_items.Count - 1)
                if (
                    quests[x].required_items[0]
                    == quests[x].required_items[quests[x].required_items.Count - 1]
                )
                    GD.PrintErr("ITEMS IN QUEST " + x + " are in duplicated use");
        }
    }

    public void RemoveQuestItems()
    {
        if (!next_quest_is_doubled_items)
        {
            foreach (Item quest_item in quests[current_quest_id].required_items)
                PlayerInventoryUI.instance.RemoveItem(
                    new Item(
                        quest_item.info,
                        (int)(quest_item.amount * (int)GameManager.difficulty_multiplier)
                    ),
                    PlayerInventoryUI.instance.inventory_items
                );
        }
        else
        {
            foreach (Item quest_item in quests[current_quest_id].required_items)
                PlayerInventoryUI.instance.RemoveItem(
                    new Item(quest_item.info, quest_item.amount * 2),
                    PlayerInventoryUI.instance.inventory_items
                );
        }
    }

    public async void NextQuest(bool finished_correctly = true, int penalty = -1)
    {
        if (finished_correctly)
        {
            QuestMenu.instance.CloseQuestMenu();
            RemoveQuestItems();
            GameManager.money += quests[current_quest_id].reward_money;
            PlayerUI.instance.UpdateMoneyLabel();

            monster_island.ApplyQuestCompleted();
        }
        else
            monster_island.ApplyQuestFailed();

        next_quest_is_doubled_items = false;

        // Nur Penalty anwenden wenn das Monster nicht bereits GameOver ausgelöst hat (mood > 0)
        if (monster_island.GetMood() > 0f)
            quest_penality.ApplyPenalty(penalty);

        if (current_quest_id == quests.Count - 1)
        {
            Debug.Print("Last Quest achieved");
            PlayerUI.LastQuestPanelShow();
            await ToSignal(PlayerUI.instance.qcp_timer, "timeout");
            PlayerUI.instance.quest_complete_panel.Visible = false;
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
            PlayerUI.CompleteQuestPanelShow();
            CutsceneManager.In_Cutscene = true;
            quest_timer.PauseTimer();
            await ToSignal(PlayerUI.instance.qcp_timer, "timeout");
            quest_timer.StartTimer();
            QuestMenu.instance.OnOpenQuestMenu();
        }

        CutsceneManager.In_Cutscene = false;
    }
}
