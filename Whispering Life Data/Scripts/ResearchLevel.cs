using System;
using Godot;

[GlobalClass]
public partial class ResearchLevel : Resource
{
    [Export]
    public string title;

    [Export]
    public string desc;

    [Export]
    public string bonus;
}
