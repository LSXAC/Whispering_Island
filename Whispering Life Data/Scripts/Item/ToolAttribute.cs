using System;
using Godot;

[GlobalClass]
public partial class ToolAttribute : WearableAttribute
{
    [Export]
    public TYPE type = TYPE.None;

    [Export]
    public PlayerStats.TYPE use_type = PlayerStats.TYPE.MINING;

    [Export]
    public MineableObject.MINING_LEVEL mining_level = MineableObject.MINING_LEVEL.Hand;

    public enum TYPE
    {
        None,
        Axe,
        Pickaxe,
        Shovel,
        Hoe,
        FishingRod,
        Hammer,
        Saw,
        Scythe,
        Shears
    }
}
