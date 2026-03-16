using System;
using System.Diagnostics;
using Godot;

public partial class MagicPowerListener : Node2D
{
    public float magical_power_generation = 0f;
    public float magical_power_consumtion = 0f;
    public float efficiency_factor = 0.1f;

    private Island island;

    public override void _Ready()
    {
        base._Ready();
        island = GetParent<Island>();
    }

    public void AddPowerConsumtion(float amount)
    {
        magical_power_consumtion += amount;
    }

    public void AddPowerGeneration(float amount)
    {
        magical_power_generation += amount;
    }

    public void ApplyPowerByPlaceableBuildings()
    {
        CalculatePower();
        UpdateBuildingsPower();
        UpdateUI();
    }

    private void CalculatePower()
    {
        magical_power_consumtion = 0;
        magical_power_generation = 0;
        //Calculate the total magical power generation and consumtion of the island
        foreach (placeable_building building in island.island_object_save_manager.GetChildren())
        {
            if (building.consums_magic_power)
                building.ApplyMagicPowerConsumtionFromManager(this);
            if (building is MagicGenerator generator)
                generator.GeneratePower(this);
        }
        CalculateEfficiencyFactor();
    }

    private void CalculateEfficiencyFactor()
    {
        if (magical_power_consumtion == 0)
        {
            efficiency_factor = 0.1f;
            return;
        }
        efficiency_factor = Math.Clamp(
            magical_power_generation / magical_power_consumtion,
            0.1f,
            1f
        );
        Debug.Print("current Efficiency Factor: " + efficiency_factor);
    }

    private void UpdateBuildingsPower()
    {
        //Update the magical power of the buildings that consume it
        foreach (placeable_building building in island.island_object_save_manager.GetChildren())
        {
            if (building.consums_magic_power)
            {
                if (building is MachineBase machineBase)
                    machineBase.UpdateBuildingsTimerEfficiencyFactor(efficiency_factor);

                building.UpdateMagicPowerBuilding(this, efficiency_factor >= 0.5f);
            }
        }
    }

    private void UpdateUI()
    {
        //Update UI if player is on the island
        if (IslandManager.instance.GetNearestIsland(Player.instance.GlobalPosition) == island)
            MagicPowerPanel.instance.UpdateMagicPowerUI(
                magical_power_consumtion,
                magical_power_generation
            );
    }
}
