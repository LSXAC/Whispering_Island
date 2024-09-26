using System;
using System.Diagnostics;
using DialogueManagerRuntime;
using Godot;

public partial class Game_Manager : Node2D
{
    [Export]
    public Building_Placer building_placer;

    public static string game_version = "a.0.1";

    public static Game_Manager INSTANCE = null;
    public static string player_name = "Player";

    public static bool inside_game_menu = false;

    public static bool[,] island_matrix = new bool[21, 21];

    public static BuildingMode building_mode = BuildingMode.None;

    public static float game_time_since_start = 0f;

    public static bool In_Cutscene = false;
    public static bool tutorial_finished = false;
    public Node2D island_parent;
    public Camera2D cutscene_camera;

    public enum BuildingMode
    {
        None,
        Placing,
        Removing
    }

    public Timer game_timer;

    private SaveState save_state = new SaveState();

    public static void SetIslandOnMatrix(int x, int y, bool state)
    {
        island_matrix[x + 10, y + 10] = state;
    }

    public static bool IsIslandOnMatrix(int x, int y)
    {
        if (island_matrix[x + 10, y + 10])
            return true;
        return false;
    }

    public override void _Ready()
    {
        INSTANCE = this;

        cutscene_camera = GetNode<Camera2D>("CutsceneCamera");
        game_timer = GetNode<Timer>("GameTimer");
        island_parent = GetNode<Node2D>("IslandManager");
        Node2D main_island = island_parent.GetNode<Node2D>("MainIsland");
        Island_Properties mi = main_island.GetNode<Node2D>("IslandProperties") as Island_Properties;

        TranslationServer.SetLocale("de");

        CreateIslands();
        CheckGameSave();
    }

    public void SaveGame()
    {
        save_state.char_save = Player.char_save;
        save_state.char_save.player_position = Player.INSTANCE.Position;

        save_state.env_save.island_Saves = Islands_Manager.INSTANCE.island_saves;

        Islands_Manager.INSTANCE.SaveResourceObjects();
        save_state.env_save.resource_object_manager_saves = Islands_Manager.INSTANCE.roms;

        //Save Quest
        save_state.quest_save.current_quest_id = QuestManager.current_quest_id;
        save_state.quest_save.quest_time_left = QuestManager.current_quest_time;
        save_state.game_time_since_start = game_time_since_start;

        save_state.char_save.inventory_items = Inventory.INSTANCE.inventory_items;

        //Save Machines and Belts
        building_placer.SavePlacedObjects();
        save_state.belt_saves = building_placer.belt_saves;
        save_state.machine_saves = building_placer.machine_saves;

        save_state.tutorial_finished = tutorial_finished;

        save_state.WriteSave();
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

        Player.char_save = save_state.char_save;
        Player.INSTANCE.Position = save_state.char_save.player_position;
        Inventory.INSTANCE.LoadInventoryFromSave(save_state.char_save.inventory_items);

        Islands_Manager.INSTANCE.island_saves = save_state.env_save.island_Saves;
        Islands_Manager.INSTANCE.LoadIslands(save_state.env_save.resource_object_manager_saves);

        //Load Machines and Belts
        building_placer.belt_saves = save_state.belt_saves;
        building_placer.machine_saves = save_state.machine_saves;
        building_placer.LoadPlacedObjects();

        // Start Quest
        QuestManager.INSTANCE.StartQuest(save_state.quest_save);

        game_time_since_start = save_state.game_time_since_start;
        game_timer.Start();
    }

    public void OnGameTimerTimeout()
    {
        game_time_since_start += 0.1f;
        player_ui.INSTANCE.UpdateGameTimeLabel();
    }

    private void CreateIslands()
    {
        Island_Properties main_island = island_parent.GetNode<Island_Properties>("MainIsland");
        SetIslandOnMatrix(0, 0, true); //Main Island
        SetIslandOnMatrix(0, -1, true); //Monster Island
        main_island.GetSigns();
    }

    private void CheckGameSave()
    {
        if (SaveState.HasSave())
            LoadGame();
        else
        {
            Debug.Print("Game_Manager - New SaveState");
            save_state = new SaveState();
            save_state.WriteSave();
            StartTutorial();
        }
    }

    private void StartTutorial()
    {
        var dialogue = GD.Load<Resource>("res://Dialogues/Tutorial.dialogue");
        Global.MoveCamera(new Vector2(0, -256));
        DialogueManager.TranslationSource = TranslationSource.CSV;
        DialogueManager.ShowDialogueBalloon(dialogue, "Tutorial");
    }
}
