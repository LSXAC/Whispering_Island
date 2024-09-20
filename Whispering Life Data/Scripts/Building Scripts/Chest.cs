using System;
using Godot;
using Godot.Collections;

public partial class Chest : MachineBase
{
    [Export]
    public ItemSave[] chest_items = new ItemSave[20];

    
}
