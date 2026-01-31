using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemSubResearchLevel : Resource
{
    public enum CATEGORY
    {
        TEXTURE,
        DENSITY,
        HARDNESS,
        COMPOSITION,
        BEHAVIOR
    }

    [Export]
    public CATEGORY category;

    [Export]
    public int time_needed = 60;
}
