using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class QuestStagePool : Resource
{
    [Export]
    public Array<QuestInfo> quests = new Array<QuestInfo>();
}
