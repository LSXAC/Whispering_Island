using System;
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

    public override void _Ready()
    {
        base._Ready();
        Timer spawn_timer = GetNode<Timer>("SpawnTimer");
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
