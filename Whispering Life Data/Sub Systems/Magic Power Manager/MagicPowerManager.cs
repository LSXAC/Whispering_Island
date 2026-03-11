using System;
using Godot;
using Godot.Collections;

public partial class MagicPowerManager : Node2D
{
    public static MagicPowerManager instance;
    public float current_magical_power = 1000f;
    public float max_magical_power = 1000f;

    public float natural_regeneration = 15f;

    public override void _Ready()
    {
        instance = this;
    }

    public float GetCurrentMagicPower()
    {
        return current_magical_power;
    }

    public void AddMagicPower(float amount)
    {
        current_magical_power += amount;

        if (current_magical_power > max_magical_power)
            current_magical_power = max_magical_power;
        PlayerUI.instance.magic_power_panel.UpdateLatestMagicPowerUI(amount);
        PlayerUI.instance.magic_power_panel.UpdateMagicPowerUI();
    }

    public bool HasEnoughMagicPower(float amount)
    {
        if (current_magical_power >= amount)
            return true;
        else
            return false;
    }

    public void ConsumeMagicPowerByPlaceableBuildings()
    {
        Array<Island> islands = IslandManager.instance.GetIslands();
        foreach (Island island in islands)
        {
            foreach (placeable_building building in island.island_object_save_manager.GetChildren())
            {
                if (building.uses_magic_power)
                    building.RemoveMagicPowerConsumtionFromManager(
                        building.magic_power_consumption
                    );
            }
        }
    }

    public void RemoveMagicPower(float amount)
    {
        current_magical_power -= amount;

        if (current_magical_power < 0)
            current_magical_power = 0;
        PlayerUI.instance.magic_power_panel.UpdateMagicPowerUI();
    }
}
