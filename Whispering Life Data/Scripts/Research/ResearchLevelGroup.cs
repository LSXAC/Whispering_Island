using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ResearchLevelGroup : Resource
{
    [Export]
    public string translation_string = "";

    [Export]
    public Array<ResearchLevel> research_levels;
}
