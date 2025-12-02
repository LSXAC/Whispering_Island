using System;
using Godot;

public partial class QuestSign : Building_Node
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void OnMouseClick()
    {
        if (GlobalFunctions.GetDistanceToPlayer(this.GlobalPosition) >= 45)
            return;

        QuestMenu.instance.OnOpenQuestMenu();
    }
}
