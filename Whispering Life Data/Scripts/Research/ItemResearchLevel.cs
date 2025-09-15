using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemResearchLevel : Resource
{
    [Export]
    public Array<ItemInfo> unlocked_item_infos_after_research;

    [Export]
    public Array<ItemSubResearchLevel> sub_levels;

    private Array<ItemInfo> getUnlockedItemInfosArray()
    {
        return unlocked_item_infos_after_research;
    }
}
