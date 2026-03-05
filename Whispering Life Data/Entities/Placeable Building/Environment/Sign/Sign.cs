using System;
using System.Diagnostics;
using Godot;

public partial class Sign : Building_Node
{
    [Export]
    public Island island;

    [Export]
    public Island.DIRECTION dir;

    public override void OnMouseClick()
    {
        if (GlobalFunctions.GetDistanceToPlayer(this.GlobalPosition) >= 20f)
            return;

        GameMenu.instance.OnOpenIslandTab();
        IslandMenu.instance.current_sign = this;

        Debug.Print("Sign: " + GameManager.island_matrix.ToString() + " was clicked!");
    }

    public void RemoveSelf()
    {
        sprite_anim_manager.shadowNode.RemoveShadow();
        QueueFree();
    }
}
