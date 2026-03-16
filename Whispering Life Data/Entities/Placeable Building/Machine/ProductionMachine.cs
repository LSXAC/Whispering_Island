using System;
using System.Diagnostics;
using Godot;

public partial class ProductionMachine : MachineBase
{
    [Export]
    public ItemInfo output_item_resource;

    [Export]
    public float spawn_time = 5f;

    [Export]
    public int count = 0;

    public int progress = 0;

    public override void _Process(double delta)
    {
        if (machine_enabled && !has_enough_magic_power)
        {
            DisableMachine();
            process_timer.Paused = true;
        }
        else if (!machine_enabled && has_enough_magic_power)
        {
            EnableMachine();
            process_timer.Paused = false;
        }
        Debug.Print("Current WaitTime: " + process_timer.WaitTime);
    }

    public void OnSpawnTimeout()
    {
        if (progress >= 100)
        {
            count++;
            progress = 0;
        }
        progress += (int)(10f / spawn_time);
        if (hover_menu.instance.current_object == this)
            hover_menu.InitHoverMenu(this);
    }

    public override void Load(Resource save)
    {
        if (save is MachineSave machine_save)
        {
            base.Load(save);
            count = machine_save.count;
        }
        else
            Logger.PrintWrongSaveType();
    }

    public override Resource Save()
    {
        MachineSave machine_save = (MachineSave)base.Save();
        machine_save.count = count;
        return machine_save;
    }
}
