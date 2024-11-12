using System;
using Godot;
using Godot.Collections;

public partial class ResearchDB : Node
{
    public static Dictionary<string, ResearchLevelManager> researchs = new Dictionary<
        string,
        ResearchLevelManager
    >()
    {
        {
            RESEARCH_ID.WOOD.ToString(),
            ResourceLoader.Load<ResearchLevelManager>("res://Buildings/Belt.tres")
        }
    };

    public enum RESEARCH_ID
    {
        WOOD,
    }
}
