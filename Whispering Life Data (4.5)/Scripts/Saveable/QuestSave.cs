using System;
using Godot;

public partial class QuestSave : Resource
{
    [Export]
    public int current_quest_id = 0;

    [Export]
    public int quest_time_left = 0;
}
