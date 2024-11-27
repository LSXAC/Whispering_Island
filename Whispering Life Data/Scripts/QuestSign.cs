using System;
using Godot;

public partial class QuestSign : Building_Node
{
    public override void OnMouseClick()
    {
        if (GlobalFunctions.GetDistanceToPlayer(this.GlobalPosition) >= 45)
            return;

        QuestMenu.INSTANCE.OnOpenQuestMenu();
    }
}
