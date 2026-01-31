using System;
using Godot;
using Godot.Collections;

public partial class ChestBase : MachineBase
{
    [Export]
    public ItemSave[] chest_items = new ItemSave[20];

    public int GetAmountOfFreeSlots()
    {
        int amount = 0;
        foreach (ItemSave i_s in chest_items)
            if (i_s == null)
                amount++;

        return amount;
    }

    public override void Load(Resource save)
    {
        if (save is MachineSave machine_save)
        {
            base.Load(machine_save);
            for (int i = 0; i < machine_save.chest_items.Length; i++)
                chest_items[i] = machine_save.chest_items[i];
        }
        else
            Logger.PrintWrongSaveType();
    }

    public override Resource Save()
    {
        MachineSave ms = (MachineSave)base.Save();
        ms.chest_items = chest_items;
        return ms;
    }
}
