using System;
using Godot;

public partial class MachineBase : placeable_building
{
    public enum MachineType
    {
        WOODFARM,
        FURNACE,
        QUARRY
    }

    [Export]
    public MachineType type;

    [Export]
    public int import_count = 0;

    [Export]
    public ItemInfo import_item_info;

    [Export]
    public int export_count = 0;

    [Export]
    public ItemInfo export_item_info;
}
