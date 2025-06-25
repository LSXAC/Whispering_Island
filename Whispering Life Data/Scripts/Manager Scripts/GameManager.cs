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
    public Building_Placer building_placer;

    public static CanvasLayer current_activ_canvaslayer = null;

    public static GameManager instance = null;
    public static string player_name = "Player";
    public static bool gameover = false;

    public static bool[,] island_matrix;

    public static BuildingMode building_mode = BuildingMode.None;

    public static float game_time_since_start;

    public static bool In_Cutscene = false;
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

    public Timer game_timer;

    public SaveState save_state = new SaveState();

    public static void SetIslandOnMatrix(int x, int y, bool state)
    {
        island_matrix[x + 10, y + 10] = state;
    }

    public static bool IsIslandOnMatrix(int x, int y)
    {
        if (x > 10 || y > 10 || x < -10 || y < -10)
            return true;

        if (island_matrix[x + 10, y + 10])
            return true;
        return false;
    }

    public override void _Ready()
    {
        instance = this;
        gameover = false;
        In_Cutscene = false;
        island_matrix = new bool[21, 21];
        current_activ_canvaslayer = null;
        game_time_since_start = 0f;

        cutscene_camera = GetNode<Camera2D>("CutsceneCamera");
        game_timer = GetNode<Timer>("GameTimer");
        island_parent = GetNode<Node2D>("IslandManager");
        Node2D main_island_scene = island_parent.GetNode<Node2D>("MainIsland");

        CreateIslands();
        CheckGameSave();
    }

    public void SaveGame()
    {
        save_state.char_save = Player.char_save;
        save_state.char_save.player_position = Player.instance.Position;
        save_state.char_save.health_value = Player.instance.player_stats.health_value;
        save_state.char_save.fatigue_value = Player.instance.player_stats.fatigue_value;

        save_state.env_save.island_Saves = IslandManager.instance.island_saves;

        IslandManager.instance.SaveResourceObjects();
        save_state.env_save.resource_object_manager_saves = IslandManager.instance.roms;
        save_state.build_saves = IslandManager.instance.build_saves;

        //Save Quest
        save_state.quest_save.current_quest_id = QuestManager.current_quest_id;
        save_state.quest_save.quest_time_left = QuestManager.current_quest_time;
        save_state.game_time_since_start = game_time_since_start;

        if (PlayerInventoryUI.instance != null)
            save_state.char_save.inventory_items = PlayerInventoryUI.instance.inventory_items;

        save_state.char_save.equipped_armor = EquipmentPanel.instance.equipped_armor;
        save_state.char_save.equipped_tool = EquipmentPanel.instance.equipped_tools;

        save_state.tutorial_finished = tutorial_finished;

        save_state.dateTime_save_string = DateTime.Now.ToString();

        save_state.char_save.research_slot_item = ResearchTab.research_slot_item;
        save_state.research_saves = ResearchTab.research_saves;

        save_state.skill_saves = Skilltree.skill_progress;
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

        ResearchTab.research_slot_item = save_state.char_save.research_slot_item;
        ResearchTab.research_saves = save_state.research_saves;

        Skilltree.skill_progress = save_state.skill_saves;
        ResearchTab.instance.Research_Points = save_state.Research_Points;

        game_time_since_start = save_state.game_time_since_start;

        HeartManager.instance.current_hearts = save_state.current_hearts;
        HeartManager.instance.UpdateHeartUI();

        // Start Quest
        QuestManager.next_quest_is_doubled_items = save_state.is_doubled_quest;
        QuestManager.instance.StartQuest(save_state.quest_save);

        game_timer.Start();
    }

    public void OnGameTimerTimeout()
    {
        game_time_since_start += 0.1f;
        PlayerUI.instance.UpdateGameTimeLabel();
    }

    private void CreateIslands()
    {
        Island main_island = island_parent.GetNode<Island>("MainIsland");
        SetIslandOnMatrix(0, 0, true); //Main Island
        SetIslandOnMatrix(0, -1, true); //Monster Island
        main_island.GetSigns();
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
        instance.game_timer.Stop();
        gameover = true;
    }
}
