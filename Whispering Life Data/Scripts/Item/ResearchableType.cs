using System;
using Godot;

[GlobalClass]
public partial class ResearchableType : ItemType
{
    [Export]
    public Database.RESEARCH_ID research_id;
}
