using System;
using Godot;

public partial class QuestSign : Building_Node
{
    [Export]
    public AnimationPlayer anim_player;

    public override void _Ready()
    {
        anim_player.Play("Moving");
    }

    public override void OnMouseClick()
    {
        if (GlobalFunctions.GetDistanceToPlayer(this.GlobalPosition) >= 45)
            return;

        QuestMenu.INSTANCE.OnOpenQuestMenu();
    }
}
