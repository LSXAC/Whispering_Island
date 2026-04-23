using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class QuestManager : Node
{
    [Export]
    public Array<QuestStagePool> quest_pool; // Pool der Quests pro Stage mittels QuestStagePool Wrapper

    private Resource quest_dialogue = ResourceLoader.Load<Resource>(
        ResourceUid.UidToPath("uid://c1p7gva6jex80")
    );
    public static int current_quest_id = 0;
    public static QuestManager instance = null;
    public static int current_quest_time = 0;

    public static QuestInfo current_selected_quest = null; // Die aktuelle ausgewählte Quest
    public static int current_master_seed = 0; // Der einfache Seed vom Nutzer
    public static bool next_quest_half_time = false;
    public static bool next_quest_is_doubled_items = false;

    [Export]
    public QuestTimer quest_timer;
    public QuestPenality quest_penality;

    private MonsterIsland monster_island;
    private MonsterIslandStateManager island_state_manager;

    [Signal]
    public delegate void FinishedGameEventHandler();

    public override void _Ready()
    {
        instance = this;
        current_quest_time = 0;
        current_quest_id = 0;

        quest_penality = GetNode<QuestPenality>("QuestPenality");
        if (monster_island != null)
            island_state_manager = monster_island.GetNode<MonsterIslandStateManager>(
                "StateManager"
            );
    }

    public void StartQuest(QuestSave quest_save = null)
    {
        Debug.Print("Start new Quest!");
        double difficultyTimeMultiplier = GetDifficultyTimeMultiplier();

        if (quest_save != null && quest_save.current_selected_quest != null)
        {
            current_quest_id = quest_save.current_quest_id;
            current_selected_quest = quest_save.current_selected_quest;
            current_master_seed = quest_save.master_seed;

            if (quest_save.quest_time_left <= 0)
            {
                QuestInfo timeReferenceQuest = GetQuestByIndex(current_quest_id, 0);
                if (timeReferenceQuest != null)
                    current_quest_time =
                        Mathf.RoundToInt(
                            timeReferenceQuest.quest_time * difficultyTimeMultiplier / 5.0
                        ) * 5;
            }
            else
                current_quest_time = quest_save.quest_time_left;
        }
        else
        {
            if (current_master_seed == 0)
            {
                current_master_seed = (int)GD.Randi();
                GD.Print($"No seed provided, auto-generated: {current_master_seed}");
            }
        }

        CheckQuestDuplications();
        quest_timer.StartTimer();

        if (GameMenu.questMenu != null)
        {
            GameMenu.questMenu.OnOpenQuestMenu();

            if (current_selected_quest == null)
            {
                GameMenu.questMenu.ShowQuestSelection();
                GameMenu.questMenu.InitQuestSelection(current_quest_id);
            }
            else
            {
                GameMenu.questMenu.ShowQuestDisplay();
                GameMenu.questMenu.InitQuest(current_selected_quest);
                QuestMiniPanel.instance.UpdateTimeLabel(current_quest_time);
                QuestMiniPanel.instance.InitQuestMiniPanel(current_selected_quest);
                MonsterIsland.instance.InitializeQuestTimers(current_quest_time);
            }
        }

        QuestMiniPanel.instance.UpdateTimeLabel(current_quest_time);
    }

    public void OnQuestTimerTimeout()
    {
        if (current_selected_quest == null)
            return;

        if (current_quest_time <= 0)
        {
            GameMenu.CloseLastWindow();
            instance.quest_timer.Stop();
            if (GameManager.gameover)
                return;

            if (CutsceneManager.skip_cutscenes)
            {
                NextQuest(finished_correctly: false, penalty: -1);
                return;
            }

            ApplyPenality();
            return;
        }

        current_quest_time -= GameManager.time_multiplier;
        QuestMiniPanel.instance.UpdateTimeLabel(current_quest_time);
    }

    public async void ApplyPenality(bool with_poisoning = false)
    {
        Debug.Print("Apply Penality!");
        int penalty = quest_penality.DeterminePenalty();
        quest_penality.PlayPenaltyCutscene(penalty, with_poisoning);
        await ToSignal(PlayerUI.instance.quest_accept_panel.confirm_button, "pressed");
        GlobalFunctions.LeaveDialogue();
        Debug.Print("End Apply Penality");
        NextQuest(finished_correctly: false, penalty);
    }

    private void CheckQuestDuplications()
    {
        for (int questIdx = 0; questIdx < quest_pool.Count; questIdx++)
        {
            if (quest_pool[questIdx] == null || quest_pool[questIdx].quests == null)
                continue;

            for (int x = 0; x < quest_pool[questIdx].quests.Count; x++)
            {
                QuestInfo quest = quest_pool[questIdx].quests[x];
                if (quest == null)
                    continue;

                for (int i = 0; i < quest.required_items.Count; i++)
                    if (i + 1 < quest.required_items.Count)
                        if (quest.required_items[i].info == quest.required_items[i + 1].info)
                            GD.PrintErr(
                                "ITEMS IN QUEST " + questIdx + "-" + x + " are in duplicated use"
                            );

                if (0 != quest.required_items.Count - 1)
                    if (
                        quest.required_items[0]
                        == quest.required_items[quest.required_items.Count - 1]
                    )
                        GD.PrintErr(
                            "ITEMS IN QUEST " + questIdx + "-" + x + " are in duplicated use"
                        );
            }
        }
    }

    public void RemoveQuestItems()
    {
        if (current_selected_quest == null)
            return;

        if (!next_quest_is_doubled_items)
        {
            foreach (Item quest_item in current_selected_quest.required_items)
                GameMenu.questMenu.quest_inventory.RemoveItem(
                    new Item(
                        quest_item.info,
                        (int)(quest_item.amount * (int)GameManager.difficulty_multiplier)
                    ),
                    GameMenu.questMenu.quest_inventory.inventory_items
                );
        }
        else
        {
            foreach (Item quest_item in current_selected_quest.required_items)
                GameMenu.questMenu.quest_inventory.RemoveItem(
                    new Item(quest_item.info, quest_item.amount * 2),
                    GameMenu.questMenu.quest_inventory.inventory_items
                );
        }

        ReturnExcessQuestItemsToInventory();
    }

    private void ReturnExcessQuestItemsToInventory()
    {
        if (GameMenu.questMenu == null || GameMenu.questMenu.quest_inventory == null)
            return;

        if (PlayerInventoryUI.instance == null)
            return;

        ItemSave[] remaining_items = GameMenu.questMenu.quest_inventory.inventory_items;

        foreach (ItemSave item_save in remaining_items)
        {
            if (item_save == null || item_save.amount <= 0)
                continue;

            ItemInfo item_info = Inventory.ITEM_TYPES[(Inventory.ITEM_ID)item_save.item_id];
            if (item_info == null)
                continue;

            Item remaining_item = new Item(
                item_info,
                item_save.amount,
                (Item.STATE)item_save.state
            );

            PlayerInventoryUI.instance.AddItem(
                remaining_item,
                PlayerInventoryUI.instance.inventory_items
            );
        }

        GameMenu.questMenu.quest_inventory.ClearInventory();
    }

    public async void FinishGame()
    {
        Debug.Print("Last Quest achieved");
        PlayerUI.LastQuestPanelShow();
        await ToSignal(PlayerUI.instance.qcp_timer, "timeout");
        PlayerUI.instance.quest_complete_panel.Visible = false;
        current_quest_id = -1;
    }

    public async void NextQuest(bool finished_correctly = true, int penalty = -1)
    {
        GameMenu.instance.OnExitButton();
        if (finished_correctly)
        {
            RemoveQuestItems();
            if (current_selected_quest != null)
            {
                GameManager.money += current_selected_quest.reward_money;
                PlayerUI.instance.UpdateMoneyLabel();
            }
            monster_island.ApplyQuestCompleted();
            MonsterIsland.instance.StartHittingCutscene();
            await ToSignal(this, SignalName.FinishedGame);
            if (MonsterIsland.instance.health_bar.current_health <= 0)
                FinishGame();
        }
        else
            monster_island.ApplyQuestFailed();

        next_quest_is_doubled_items = false;

        if (monster_island.GetMood() > 0f)
            quest_penality.ApplyPenalty(penalty);

        if (current_quest_id == quest_pool.Count - 1)
        {
            Debug.Print("Last Quest achieved");
            FinishGame();
        }

        current_quest_id++;
        current_selected_quest = null;
        StartQuest();

        if (next_quest_half_time)
        {
            current_quest_time = (int)(current_quest_time / 2.0);
            MonsterIsland.instance.InitializeQuestTimers(current_quest_time);
            next_quest_half_time = false;
        }

        QuestMiniPanel.instance.UpdateTimeLabel(current_quest_time);
        if (finished_correctly)
        {
            CutsceneManager.In_Cutscene = true;
            quest_timer.PauseTimer();
            await ToSignal(PlayerUI.instance.qcp_timer, "timeout");
            quest_timer.StartTimer();
            GameMenu.questMenu.OnOpenQuestMenu();
        }

        CutsceneManager.In_Cutscene = false;
    }

    private double GetDifficultyTimeMultiplier()
    {
        return 1.0 / GameManager.difficulty_multiplier;
    }

    public QuestInfo GetQuestByIndex(int questId, int poolIndex)
    {
        if (questId < 0 || questId >= quest_pool.Count)
            return null;

        if (quest_pool[questId] == null || quest_pool[questId].quests == null)
            return null;

        if (poolIndex < 0 || poolIndex >= quest_pool[questId].quests.Count)
            return null;

        return quest_pool[questId].quests[poolIndex];
    }

    public Array<QuestInfo> GetRandomQuestsFromPool(int questId, int selectCount = 3)
    {
        Array<QuestInfo> selectedQuests = new Array<QuestInfo>();

        if (questId < 0 || questId >= quest_pool.Count)
            return selectedQuests;

        if (quest_pool[questId] == null || quest_pool[questId].quests == null)
            return selectedQuests;

        Array<QuestInfo> poolQuests = quest_pool[questId].quests;

        if (poolQuests.Count == 0)
            return selectedQuests;

        if (poolQuests.Count <= selectCount)
        {
            foreach (QuestInfo quest in poolQuests)
                selectedQuests.Add(quest);
            return selectedQuests;
        }

        int seedForThisStage =
            (current_master_seed != 0) ? current_master_seed + questId : (int)GD.Randi();

        Random rnd = new Random(seedForThisStage);
        Array<int> availableIndices = new Array<int>();

        for (int i = 0; i < poolQuests.Count; i++)
            availableIndices.Add(i);

        for (int i = 0; i < selectCount; i++)
        {
            int randomIdx = rnd.Next(0, availableIndices.Count);
            int questIdx = availableIndices[randomIdx];
            selectedQuests.Add(poolQuests[questIdx]);
            availableIndices.RemoveAt(randomIdx);
        }

        return selectedQuests;
    }

    public void GenerateSeedsFromMasterSeed(int masterSeed)
    {
        current_master_seed = masterSeed;
        GD.Print($"Seed gesetzt: {masterSeed}");
    }

    public void SetSeedFromInput(string seedInput)
    {
        if (string.IsNullOrWhiteSpace(seedInput))
        {
            current_master_seed = (int)GD.Randi();
            GD.Print($"No seed provided, auto-generated: {current_master_seed}");
        }
        else
        {
            if (int.TryParse(seedInput, out int parsedSeed))
            {
                current_master_seed = parsedSeed;
                GD.Print($"Seed gesetzt: {parsedSeed}");
            }
            else
            {
                GD.PrintErr($"Invalid seed input: '{seedInput}'. Must be a number!");
                current_master_seed = (int)GD.Randi();
                GD.Print($"No seed provided, auto-generated: {current_master_seed}");
            }
        }
    }
}
