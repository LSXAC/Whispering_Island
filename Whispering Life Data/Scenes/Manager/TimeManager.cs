using System;
using Godot;

public partial class TimeManager : Node2D
{
    [Export]
    public Timer game_timer;

    public static TimeManager instance;
    public int current_game_time = 360; // Start at 6:00 AM (360 minutes)
    public int current_day = 0;

    public override void _Ready()
    {
        instance = this;
    }

    public void OnGameTimerTimeout()
    {
        UpdatePlayerUITime();
        DayNightManager.instance.UpdateColor();
        current_game_time += GameManager.time_multiplier; // Increment game time by 5 minutes
        if (CheckIfNewDay())
        {
            current_game_time = 0;
            current_day++;
            // Handle new day logic here, e.g., reset daily quests, update UI, etc.
            GD.Print("New day started! Current day: " + current_day);
        }
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
        int minutes = (current_game_time % 60); // 5 minutes per game hour
        string formattedTime = $"{hours:D2}:{minutes:D2}";
        return formattedTime;
    }

    public void UpdatePlayerUITime()
    {
        PlayerUI.instance.UpdateGameTimeLabel();
    }
}
