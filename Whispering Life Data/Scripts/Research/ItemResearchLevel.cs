using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemResearchLevel : Resource
{
    [Export]
    public Array<ItemInfo> items_unlocked_after_research;

    private Array<ItemInfo> getUnlockedItemInfosArray()
    {
        return items_unlocked_after_research;
    }
}
