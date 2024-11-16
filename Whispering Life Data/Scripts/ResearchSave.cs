using System;
using Godot;
using Godot.Collections;

public partial class ResearchSave : Resource
{
    public ResearchSave() { }

    public bool AddLevel(Database.UPGRADE_LEVEL level)
    {
        if (research_level < (int)level)
        {
            research_level++;
            return true;
        }
        return false;
    }

    [Export]
    public int research_level = 0;
}
