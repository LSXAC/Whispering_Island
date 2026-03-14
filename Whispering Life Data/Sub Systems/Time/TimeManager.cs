using System;
using Godot;

public partial class TimeManager : Node2D
{
    [Export]
    public Timer game_timer;

    public static TimeManager instance;
    public DayNightManager day_night_manager;
    public int current_game_time = 360; // Start at 6:00 AM (360 minutes)
    public static float light_factor = 0f; // Default light factor
    public int current_day = 0;

    public override void _Ready()
    {
        instance = this;
        day_night_manager = GetNode<DayNightManager>("DayNightManager");
        if (Logger.NodeIsNotNull(day_night_manager))
        {
            PlayerUI.instance.time_stripe.SetStripe();
            day_night_manager.UpdateColor();
            UpdatePlayerUITime();
        }
        else
            game_timer.Stop();
    }

    public void OnGameTimerTimeout()
    {
        if (PlayerInventoryUI.instance != null && Skilltree.instance != null)
            if (Skilltree.instance.HasBigSkill(SkillData.TYPE_CATEGORY.TOOL_REGENERATION))
                PlayerInventoryUI.instance.AddDurabilityToItemsThroughAutoRepair();

        day_night_manager.UpdateColor();
        current_game_time += GameManager.time_multiplier;

        if (current_game_time >= 1140 && current_game_time <= 1260)
            light_factor = (current_game_time - 1140) / 120f;
        else if (current_game_time > 1260 || current_game_time < 180)
            light_factor = 1f;
        else if (current_game_time >= 180 && current_game_time <= 300)
            light_factor = 1 - (current_game_time - 180) / 120f;
        else
            light_factor = 0f;

        if (CheckIfNewDay())
        {
            current_game_time = 0;
            current_day++;
        }

        //natural Regeneration for Magic Power
        foreach (Island island in IslandManager.instance.GetIslands())
            island.magic_power_listener.ApplyPowerByPlaceableBuildings();

        UpdatePlayerUITime();

        // GameManager.game_time_since_start = current_game_time;
        // QuestManager.current_quest_time = current_game_time;
    }

    private bool CheckIfNewDay()
    {
        if (current_game_time >= 1440) // 5/s = 1440 minutes in a day
            return true;

        return false;
    }

    public string GetTimeFormat()
    {
        int hours = current_game_time / 60;
        int minutes = current_game_time % 60; // 5 minutes per game hour
        string formattedTime = $"{hours:D2}:{minutes:D2}";
        return formattedTime;
    }

    public void UpdatePlayerUITime()
    {
        PlayerUI.instance.UpdateGameTimeLabel();
    }

    public static void PauseTime()
    {
        instance.game_timer.Stop();
        QuestManager.instance.quest_timer.PauseTimer();
    }

    public static void ResumeTime()
    {
        instance.game_timer.Start();
        QuestManager.instance.quest_timer.StartTimer();
    }
}
