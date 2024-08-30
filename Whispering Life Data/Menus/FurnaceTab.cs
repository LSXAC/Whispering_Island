using System;
using Godot;

public partial class FurnaceTab : ColorRect
{
    [Export]
    public Slot export_slot;

    [Export]
    public Slot import_slot;

    [Export]
    public Slot fuel_slot;
    public ProcessBuilding process_building;
    public static FurnaceTab INSTANCE = null;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    public void SetProcessBuilding(ProcessBuilding process_building)
    {
        this.process_building = process_building;
    }

    public void ClearProcessBuilding()
    {
        process_building = null;
    }

    public void OnVisiblityChange()
    {
        ClearProcessBuilding();
    }
}
