using System;
using Godot;

public partial class MagicPowerListener : Node2D
{
    public float magical_power_generation = 0f;
    public float magical_power_consumtion = 0f;

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
        magical_power_consumtion = 0;
        magical_power_generation = 0;
        foreach (placeable_building building in island.island_object_save_manager.GetChildren())
        {
            if (building.consums_magic_power)
                building.ApplyMagicPowerConsumtionFromManager(this);
            if (building is MagicGenerator generator)
                generator.GeneratePower(this);
        }

        bool has_enough_magic_power = magical_power_consumtion <= magical_power_generation;
        foreach (placeable_building building in island.island_object_save_manager.GetChildren())
        {
            if (building.consums_magic_power)
                building.UpdateMagicPowerBuilding(has_enough_magic_power);
        }

        if (IslandManager.instance.GetNearestIsland(Player.instance.GlobalPosition) == island)
            MagicPowerPanel.instance.UpdateMagicPowerUI(
                magical_power_consumtion,
                magical_power_generation
            );
    }
}
