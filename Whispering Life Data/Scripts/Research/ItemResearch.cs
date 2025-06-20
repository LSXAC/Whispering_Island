using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemResearch : Resource
{
    [Export]
    public Array<ItemResearchLevel> item_research_levels;

    [Export]
    public string translation_string = "";

    public ItemResearchLevel getResearchLevelByIndex(int index)
    {
        if (index >= getCountOfResearchLevels())
            return null;

        return item_research_levels[index];
    }

    public int getCountOfResearchLevels()
    {
        return item_research_levels.Count;
    }
}
