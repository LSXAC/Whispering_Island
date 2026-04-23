using System;
using Godot;
using Godot.Collections;

public partial class QuestSave : Resource
{
    [Export]
    public int current_quest_id = 0;

    [Export]
    public int quest_time_left = 0;

    [Export]
    public QuestInfo current_selected_quest = null;

    [Export]
    public Array<int> quest_pool_seeds = new Array<int>();

    [Export]
    public int master_seed = 0;
}
