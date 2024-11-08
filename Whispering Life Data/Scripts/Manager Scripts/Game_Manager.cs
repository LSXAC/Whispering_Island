using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using DialogueManagerRuntime;
using Godot;
using Godot.Collections;

public partial class Game_Manager : Node2D
{
    [Export]
    public Building_Placer building_placer;

    public static Dictionary<string, BuildingType> buildings = new Dictionary<
        string,
        BuildingType
    >()
    {
        {
            BUILDING_ID.BELT.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Belt.tres")
        },
        {
            BUILDING_ID.BELTTUNNEL.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Belt_Tunnel.tres")
        },
        {
            BUILDING_ID.CHEST.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Chest.tres")
        },
        {
            BUILDING_ID.FURNACE.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Furnace.tres")
        },
        {
            BUILDING_ID.TREE_GROWTHER.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Tree_Growther.tres")
        },
        {
            BUILDING_ID.QUARRY.ToString(),
            ResourceLoader.Load<BuildingType>("res://Buildings/Quarry.tres")
        },
    };

    public enum BUILDING_ID
    {
        BELT,
        BELTTUNNEL,
        CHEST,
        FURNACE,
        QUARRY,
        TREE_GROWTHER
    }

    public static string game_version = "a.0.1";

    public static Game_Manager INSTANCE = null;
    public static string player_name = "Player";
    public static bool gameover = false;

    public static bool inside_game_menu = false;

    public static bool[,] island_matrix;

    public static BuildingMode building_mode = BuildingMode.None;

    public static float game_time_since_start;

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

    public BuildingType GetBuildingType(BUILDING_ID building_id)
    {
        if (buildings.ContainsKey(building_id.ToString()))
            return buildings[building_id.ToString()];

        GD.PrintErr("No Building found for: " + building_id.ToString());
        return null;
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

    public override void _Ready()
    {
        INSTANCE = this;
        gameover = false;
        tutorial_finished = false;
        In_Cutscene = false;
        island_matrix = new bool[21, 21];
        inside_game_menu = false;
        game_time_since_start = 0f;

        cutscene_camera = GetNode<Camera2D>("CutsceneCamera");
        game_timer = GetNode<Timer>("GameTimer");
        island_parent = GetNode<Node2D>("IslandManager");
        Node2D main_island = island_parent.GetNode<Node2D>("MainIsland");
        Island_Properties mi = main_island.GetNode<Node2D>("IslandProperties") as Island_Properties;

        TranslationServer.SetLocale("en");

        CreateIslands();
        CheckGameSave();
    }

    public void SaveGame()
    {
        save_state.char_save = Player.char_save;
        save_state.char_save.player_position = Player.INSTANCE.Position;
        save_state.char_save.health_value = Player.INSTANCE.player_stats.health_value;
        save_state.char_save.fatigue_value = Player.INSTANCE.player_stats.fatigue_value;

        save_state.env_save.island_Saves = Islands_Manager.INSTANCE.island_saves;

        Islands_Manager.INSTANCE.SaveResourceObjects();
        save_state.env_save.resource_object_manager_saves = Islands_Manager.INSTANCE.roms;
        save_state.current_language = TranslationServer.GetLocale();

        //Save Quest
        save_state.quest_save.current_quest_id = QuestManager.current_quest_id;
        save_state.quest_save.quest_time_left = QuestManager.current_quest_time;
        save_state.game_time_since_start = game_time_since_start;

        save_state.char_save.inventory_items = Inventory.INSTANCE.inventory_items;
        save_state.char_save.equipped_armor = EquipmentPanel.INSTANCE.equipped_armor;
        save_state.char_save.equipped_tool = EquipmentPanel.INSTANCE.equipped_tools;

        //Save Machines and Belts
        building_placer.SavePlacedObjects();
        save_state.belt_saves = building_placer.belt_saves;
        save_state.machine_saves = building_placer.machine_saves;
        save_state.belt_transmitter_saves = building_placer.belt_transmitter_saves;

        save_state.tutorial_finished = tutorial_finished;

        save_state.dateTime_save_string = DateTime.Now.ToString();

        //Audio
        save_state.master_volume = AudioServer.GetBusVolumeDb(
            AudioServer.GetBusIndex(SoundSlider.BUS.Master.ToString())
        );
        save_state.music_volume = AudioServer.GetBusVolumeDb(
            AudioServer.GetBusIndex(SoundSlider.BUS.Music.ToString())
        );
        save_state.sfx_volume = AudioServer.GetBusVolumeDb(
            AudioServer.GetBusIndex(SoundSlider.BUS.SFX.ToString())
        );
        save_state.WriteSave();
        Debug.Print("Saved Game!");
    }

    public void LoadGame()
    {
        Debug.Print("Game_Manager - Loading SaveState");
        save_state = (SaveState)SaveState.LoadSave();
        TranslationServer.SetLocale(save_state.current_language);
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
        Player.INSTANCE.Position = save_state.char_save.player_position;
        Player.INSTANCE.player_stats.health_value = save_state.char_save.health_value;
        Player.INSTANCE.player_stats.fatigue_value = save_state.char_save.fatigue_value;
        Inventory.INSTANCE.LoadInventoryFromSave(save_state.char_save.inventory_items);

        EquipmentPanel.INSTANCE.LoadArmorFromSave(save_state.char_save.equipped_armor);
        EquipmentPanel.INSTANCE.LoadToolFromSave(save_state.char_save.equipped_tool);
        EquipmentPanel.INSTANCE.CalculateStatsFromEquipment();

        Islands_Manager.INSTANCE.island_saves = save_state.env_save.island_Saves;
        Islands_Manager.INSTANCE.LoadIslands(save_state.env_save.resource_object_manager_saves);

        //Load Machines and Belts
        building_placer.belt_saves = save_state.belt_saves;
        building_placer.belt_transmitter_saves = save_state.belt_transmitter_saves;
        building_placer.machine_saves = save_state.machine_saves;
        building_placer.LoadPlacedObjects();

        // Start Quest
        QuestManager.INSTANCE.StartQuest(save_state.quest_save);

        game_time_since_start = save_state.game_time_since_start;
        game_timer.Start();

        //Audio
        AudioServer.SetBusVolumeDb(
            AudioServer.GetBusIndex(SoundSlider.BUS.Master.ToString()),
            save_state.master_volume
        );
        AudioServer.SetBusVolumeDb(
            AudioServer.GetBusIndex(SoundSlider.BUS.Music.ToString()),
            save_state.music_volume
        );
        AudioServer.SetBusVolumeDb(
            AudioServer.GetBusIndex(SoundSlider.BUS.SFX.ToString()),
            save_state.sfx_volume
        );
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
            NewGame();
            StartTutorial();
        }
    }

    public void NewGame(bool skip_tutorial = false)
    {
        Debug.Print("Game_Manager - New SaveState");
        GetTree().ReloadCurrentScene();
        save_state = new SaveState();
        if (skip_tutorial)
            save_state.tutorial_finished = true;
        save_state.WriteSave();
    }

    private void StartTutorial()
    {
        Debug.Print("Start Tutorial");
        var dialogue = GD.Load<Resource>("res://Dialogues/Tutorial.dialogue");
        Global.MoveCamera(new Vector2(0, -256));
        DialogueManager.TranslationSource = TranslationSource.CSV;
        DialogueManager.ShowDialogueBalloon(dialogue, "Tutorial");
    }

    public void GameOver()
    {
        player_ui.INSTANCE.gameover_panel.Visible = true;
        gameover = true;
    }

    public void LoadIslandResources()
    {
        //foreach (Island_Properties ip in GetTree().GetNodesInGroup("Island_Properties"))
        //ip.ost.SetObjectsOnTilemap();
    }
}
