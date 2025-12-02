using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using DialogueManagerRuntime;
using Godot;
using Godot.Collections;

public partial class GameManager : Node2D
{
    [Export]
    public BuildingPlacer building_placer;

    public static CanvasLayer current_activ_canvaslayer = null;
    public static bool dev_build_mode = false;

    public static GameManager instance = null;
    public static string player_name = "Player";
    public static bool gameover = false;

    public static bool[,] island_matrix;
    public static int time_multiplier = 5;

    public static BuildingMode building_mode = BuildingMode.None;

    public static bool In_Cutscene = false;
    public static int money = 0;
    public bool tutorial_finished = false;
    public Node2D island_parent;
    public Camera2D cutscene_camera;
    public bool new_game = false;

    public enum BuildingMode
    {
        None,
        Placing,
        Removing
    }

    public SaveState save_state = new SaveState();

    public static void SetIslandOnMatrix(int x, int y, bool state)
    {
        island_matrix[x + 5, y + 5] = state;
    }

    public static int GetIslandCountOnMatrix()
    {
        if (island_matrix == null)
            return 0;
        int count = 0;
        for (int x = 0; x < 11; x++)
        for (int y = 0; y < 11; y++)
            if (island_matrix[x, y])
                count++;
        return count;
    }

    public static bool IsIslandOnMatrix(int x, int y)
    {
        if (x > 5 || y > 5 || x < -5 || y < -5)
            return true;

        if (island_matrix[x + 5, y + 5])
            return true;
        return false;
    }

    public static Island GetIslandOnMatrixOrNull(int x, int y)
    {
        Array<Island> islands = new Array<Island>();
        foreach (Node node in IslandManager.instance.GetChildren())
        {
            if (node is Island island)
                islands.Add(island);
        }
        foreach (Island island in islands)
        {
            if (island.matrix_x == x && island.matrix_y == y)
                return island;
        }
        return null;
    }

    public override void _Ready()
    {
        Input.UseAccumulatedInput = false;
        instance = this;
        gameover = false;
        In_Cutscene = false;
        island_matrix = new bool[11, 11];
        money = 0;
        current_activ_canvaslayer = null;

        cutscene_camera = GetNode<Camera2D>("CutsceneCamera");
        island_parent = GetNode<Node2D>("IslandManager");
        Node2D main_island_scene = island_parent.GetNode<Node2D>("MainIsland");

        CreateIslands();
        CheckGameSave();
    }

    public void SaveGame()
    {
        save_state.char_save = Player.char_save;
        save_state.money = money;
        save_state.char_save.player_position = Player.instance.Position;
        save_state.char_save.health_value = Player.instance.player_stats.health_value;
        save_state.char_save.fatigue_value = Player.instance.player_stats.fatigue_value;

        save_state.env_save.island_Saves = IslandManager.instance.island_saves;

        IslandManager.instance.SaveResourceObjects();
        save_state.env_save.resource_object_manager_saves = IslandManager.instance.roms;
        save_state.build_saves = IslandManager.instance.build_saves;
        save_state.island_types_build = IslandManager.instance.island_types_build;

        //Save Quest
        save_state.quest_save.current_quest_id = QuestManager.current_quest_id;
        save_state.quest_save.quest_time_left = QuestManager.current_quest_time;

        if (PlayerInventoryUI.instance != null)
            save_state.char_save.inventory_items = PlayerInventoryUI.instance.inventory_items;

        save_state.char_save.equipped_armor = EquipmentPanel.instance.equipped_armor;
        save_state.char_save.equipped_tool = EquipmentPanel.instance.equipped_tools;

        save_state.tutorial_finished = tutorial_finished;

        save_state.dateTime_save_string = DateTime.Now.ToString();
        save_state.current_days = TimeManager.instance.current_day;
        save_state.current_game_time = TimeManager.instance.current_game_time;

        save_state.char_save.research_slot_item = ResearchTab.research_slot_item;
        save_state.research_saves = ResearchTab.research_saves;

        save_state.in_research = ResearchTab.in_research;
        save_state.current_research_prog = ResearchTab.current_research_prog;
        save_state.current_selected_item_id = ResearchTab.current_selected_item_id;
        save_state.selected_sub_id = ResearchTab.selected_sub_id;

        save_state.discovered_items = DiscoverManager.discovered_items;

        save_state.skill_saves = Skilltree.instance.skill_progress;

        save_state.Research_Points = ResearchTab.instance.Research_Points;

        save_state.current_hearts = HeartManager.instance.current_hearts;
        save_state.is_doubled_quest = QuestManager.next_quest_is_doubled_items;

        save_state.WriteSave();
        MainMenu.SaveLauncherConfig();
        Debug.Print("Saved Game!");
    }

    public void LoadGame()
    {
        Debug.Print("Game_Manager - Loading SaveState");
        save_state = (SaveState)SaveState.LoadSave();
        tutorial_finished = save_state.tutorial_finished;
        if (!tutorial_finished)
        {
            StartTutorial();
            return;
        }
        try
        {
            SaveLoadTab.dateTime_from_save = DateTime.Parse(save_state.dateTime_save_string);
        }
        catch (Exception e)
        {
            Debug.Print(e.StackTrace.ToString());
            SaveLoadTab.dateTime_from_save = DateTime.Now;
        }

        Player.char_save = save_state.char_save;

        money = save_state.money;
        PlayerUI.instance.UpdateMoneyLabel();
        Player.instance.Position = save_state.char_save.player_position;
        Player.instance.player_stats.health_value = save_state.char_save.health_value;
        Player.instance.player_stats.fatigue_value = save_state.char_save.fatigue_value;
        PlayerInventoryUI.instance.LoadInventoryFromSave(save_state.char_save.inventory_items);

        EquipmentPanel.instance.LoadArmorFromSave(save_state.char_save.equipped_armor);
        EquipmentPanel.instance.LoadToolFromSave(save_state.char_save.equipped_tool);
        EquipmentPanel.instance.CalculateStatsFromEquipment();

        IslandManager.instance.island_saves = save_state.env_save.island_Saves;
        IslandManager.instance.build_saves = save_state.build_saves;
        IslandManager.instance.LoadIslands(save_state.env_save.resource_object_manager_saves);
        IslandManager.instance.island_types_build = save_state.island_types_build;

        ResearchTab.research_slot_item = save_state.char_save.research_slot_item;
        ResearchTab.research_saves = save_state.research_saves;

        ResearchTab.in_research = save_state.in_research;
        ResearchTab.current_research_prog = save_state.current_research_prog;
        ResearchTab.current_selected_item_id = save_state.current_selected_item_id;
        ResearchTab.selected_sub_id = save_state.selected_sub_id;

        DiscoverManager.discovered_items = save_state.discovered_items;

        if (Skilltree.instance.skill_datas.Count == save_state.skill_saves.Length)
            Skilltree.instance.skill_progress = save_state.skill_saves;
        else
        {
            int[] skill_progress = new int[Skilltree.instance.skill_datas.Count];
            for (int i = 0; i < save_state.skill_saves.Length; i++)
                skill_progress[i] = save_state.skill_saves[i];
            Skilltree.instance.skill_progress = skill_progress;
        }

        ResearchTab.instance.Research_Points = save_state.Research_Points;

        HeartManager.instance.current_hearts = save_state.current_hearts;
        HeartManager.instance.UpdateHeartUI();

        // Start Quest
        QuestManager.next_quest_is_doubled_items = save_state.is_doubled_quest;
        QuestManager.instance.StartQuest(save_state.quest_save);

        TimeManager.instance.current_day = save_state.current_days;
        TimeManager.instance.current_game_time = save_state.current_game_time;
        TimeManager.instance.game_timer.Start();
        TimeManager.instance.day_night_manager.UpdateColor();
        TimeManager.instance.UpdatePlayerUITime();
    }

    private void CreateIslands()
    {
        Island main_island = island_parent.GetNode<Island>("MainIsland");
        SetIslandOnMatrix(0, 0, true); //Main Island
        SetIslandOnMatrix(0, -1, true); //Monster Island
    }

    private void CheckGameSave()
    {
        if (!new_game)
            if (SaveState.HasSave())
            {
                LoadGame();
                return;
            }

        NewGame();
        if (!tutorial_finished)
            StartTutorial();
        else
        {
            GlobalFunctions.StartAfterTutorial();
            SaveGame();
            MainMenu.SaveLauncherConfig();
        }
    }

    public void NewGame(bool skip_tutorial = false)
    {
        new_game = false;
        Debug.Print("Game_Manager - New SaveState");
        save_state = new SaveState();
        if (skip_tutorial)
            save_state.tutorial_finished = true;
        save_state.WriteSave();
        //LoadGame();
    }

    private void StartTutorial()
    {
        Debug.Print("Start Tutorial");
        var dialogue = GD.Load<Resource>("res://Dialogues/Tutorial_General.dialogue");
        GlobalFunctions.MoveCameraToPosition(new Vector2(0, -256));
        GlobalFunctions.InDialogue();
        DialogueManager.TranslationSource = TranslationSource.CSV;
        DialogueManager.ShowExampleDialogueBalloon(dialogue, "Tutorial");
        return;
    }

    public void GameOver()
    {
        PlayerUI.instance.gameover_panel.Visible = true;
        GameMenu.instance.Visible = false;
        BuildMenu.instance.Visible = false;
        QuestMenu.instance.Visible = false;
        TimeManager.instance.game_timer.Stop();
        gameover = true;
    }
}
