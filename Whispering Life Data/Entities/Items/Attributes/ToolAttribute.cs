using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ToolAttribute : WearableAttribute
{
    [Export]
    public TYPE type = TYPE.None;

    [Export]
    public PlayerStats.TOOLTYPE tool_type = PlayerStats.TOOLTYPE.MINING;

    [Export]
    public MineableObject.MINING_LEVEL mining_level = MineableObject.MINING_LEVEL.HAND;

    [Export]
    public bool has_menu = false;

    [Export]
    public Array<placeable_building.TILETYPE> can_be_used_on_tile_types =
        new Array<placeable_building.TILETYPE>();

    [Export]
    public Array<placeable_building.TILETYPE> can_be_removed_on_tile_types =
        new Array<placeable_building.TILETYPE>();

    [Export]
    public int auto_tile_id = 0;

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
