using System;
using System.Diagnostics;
using Godot;

public partial class Game_Manager : Node2D
{
    public static string game_version = "a.0.1";

    public static Node2D island_parent;
    public static bool InsideGameMenu = false;

    [Export]
    public TileMap outside_tilemap;

    public static bool[,] island_matrix = new bool[21, 21];

    public island_menu island_menu;

    public static bool in_building_mode = false;

    public static float min_distance_to_interactable = 5f;

    public Timer game_timer;
    public static float game_time_since_start = 0f;

    // Called when the node enters the scene tree for the first time.

    public SaveState save_state = new SaveState();
    public static Game_Manager INSTANCE = null;

    public override void _Ready()
    {
        INSTANCE = this;

        game_timer = GetNode<Timer>("GameTimer");
        TranslationServer.SetLocale("de");
        island_parent = GetNode<Node2D>("IslandManager");
        CreateIslands();
        Node2D main_island = island_parent.GetNode<Node2D>("MainIsland");
        Island_Properties mi = main_island.GetNode<Node2D>("IslandProperties") as Island_Properties;
        CheckGameSave();
    }

    private void CheckGameSave()
    {
        if (SaveState.HasSave())
            LoadGame();
        else
        {
            Debug.Print("GM - New SaveState");
            save_state = new SaveState();
            QuestManager.INSTANCE.StartQuest();
            save_state.WriteSave();
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.N))
        { /*DEBUGS*/
        }
    }

    public void SaveGame()
    {
        Inventory.SaveInventory();
        save_state.char_save = Inventory.char_save;
        save_state.char_save.player_position = Player.instance.Position;

        save_state.env_save.island_Saves = Islands_Manager.INSTANCE.island_saves;

        Islands_Manager.INSTANCE.SaveResourceObjects();
        save_state.env_save.resource_object_manager_saves = Islands_Manager.INSTANCE.roms;

        save_state.quest_save.current_quest_id = QuestManager.current_quest_id;
        save_state.quest_save.quest_time_left = QuestManager.current_quest_time;
        save_state.game_time_since_start = game_time_since_start;
        save_state.WriteSave();
        Debug.Print("Saved Game!");
    }

    public void LoadGame()
    {
        Debug.Print("GM - Loading SaveState");
        save_state = (SaveState)SaveState.LoadSave();
        Inventory.char_save = save_state.char_save;
        Player.instance.Position = save_state.char_save.player_position;
        Inventory.LoadInventory();

        Islands_Manager.INSTANCE.island_saves = save_state.env_save.island_Saves;
        Islands_Manager.INSTANCE.LoadIslands(save_state.env_save.resource_object_manager_saves);

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

    public static void ResetMatrix()
    {
        for (int i = 0; i < island_matrix.Length; i++)
        for (int j = 0; j < island_matrix.Length; j++)
            island_matrix[i, j] = false;
    }
}
