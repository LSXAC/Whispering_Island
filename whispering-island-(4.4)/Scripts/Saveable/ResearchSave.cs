using System;
using Godot;
using Godot.Collections;

public partial class ResearchSave : Resource
{
    public ResearchSave() { }

    [Export]
    public int research_level = 0;

    [Export]
    public Dictionary<ItemSubResearchLevel.CATEGORY, bool> sub_level_progress =
        new Dictionary<ItemSubResearchLevel.CATEGORY, bool>();
}
