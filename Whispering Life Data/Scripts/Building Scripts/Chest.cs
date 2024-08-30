using System;
using Godot;
using Godot.Collections;

public partial class Chest : MachineBase
{
    [Export]
    public Array<ItemSave> chest_items = new Array<ItemSave>();
}
