/*
  Copyright 2024: Leon Schröder Asset Creations
  Website: Leon-Schroeder.net
  Project: Whispering Island
*/

using System;
using Godot;
using Godot.Collections;

public partial class Player_Stats : Node2D
{
    public float fatigue_value = 0f;
    public int health_value = 100;
    public const float fatigue_amount = 0.1f;
    public const float fatigue_remove_by_sleep_amount = 0.1f;

    public float[] stat_amounts = new float[Enum.GetNames(typeof(StatsPanel.stat_types)).Length];

    public override void _Ready() { }

    public void UpdateStatsByEquipmentChange()
    {
        //EquipmentPanel.INSTANCE
    }

    public void AddFatigue(float amount)
    {
        if (fatigue_value <= 100f)
            fatigue_value += amount;
        else
            fatigue_value = 100f;
    }

    public void RemoveFatigue(int seconds)
    {
        float amount = seconds * fatigue_remove_by_sleep_amount;
        if (fatigue_value - amount <= 0)
            fatigue_value = 0;
        else
            fatigue_value = -amount;
    }
}
