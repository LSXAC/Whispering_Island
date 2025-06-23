using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemResearchLevel : Resource
{
    [Export]
    public Array<ItemInfo> unlocked_item_resources_after_research;

    private Array<ItemInfo> getUnlockedItemInfosArray()
    {
        return unlocked_item_resources_after_research;
    }
}
