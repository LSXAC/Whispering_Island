/*
  Copyright 2024: Leon Schröder Asset Creations
  Website: Leon-Schroeder.net
*/

using System;
using Godot;
using Godot.Collections;

public partial class PlayerStats : Node2D
{
    public float fatigue_value = 0f;
    public int health_value = 100;
    public const float fatigue_amount = 0.1f;
    public const float fatigue_remove_by_sleep_amount = 0.1f;

    float[] stat_amounts = new float[5] { 1f, 1f, 1f, 1f, 1f };

    public enum TYPE
    {
        ATTACK,
        DEFENSE,
        FORESTRY,
        MINING,
        FARMING
    };

    public override void _Ready() { }

    public void IncreaseStat(TYPE type, float amount)
    {
        stat_amounts[(int)type] += amount;
    }

    public void SetStat(TYPE type, float amount)
    {
        if (amount < 1f)
            amount = 1f;

        stat_amounts[(int)type] = amount;
    }

    public float GetStatAmount(TYPE type)
    {
        return stat_amounts[(int)type];
    }

    public int GetStatsLength()
    {
        return stat_amounts.Length;
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
