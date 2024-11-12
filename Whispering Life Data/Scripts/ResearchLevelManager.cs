using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ResearchLevelManager : Resource
{
    [Export]
    public Array<ResearchLevel> research_levels;
}
