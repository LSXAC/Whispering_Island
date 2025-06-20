using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemResearchLevel : Resource
{
    [Export]
    public Array<ItemResource> unlocked_item_resources_after_research;

    private Array<ItemResource> getUnlockedItemInfosArray()
    {
        return unlocked_item_resources_after_research;
    }
}
