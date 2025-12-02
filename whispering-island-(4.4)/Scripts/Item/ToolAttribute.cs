using System;
using Godot;

[GlobalClass]
public partial class ToolAttribute : WearableAttribute
{
    [Export]
    public TYPE type = TYPE.None;

    [Export]
    public PlayerStats.TOOLTYPE tool_type = PlayerStats.TOOLTYPE.MINING;

    [Export]
    public MineableObject.MINING_LEVEL mining_level = MineableObject.MINING_LEVEL.HAND;

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

    public override string GetNameOfAttribute()
    {
        return TranslationServer.Translate("TOOL") + "\n";
    }
}
