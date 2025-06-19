using System;
using System.Diagnostics;
using Godot;

public partial class Sign : Building_Node
{
    [Export]
    public Island island_info;

    [Export]
    public Island.DIRECTION dir;

    public override void OnMouseClick()
    {
        if (GlobalFunctions.GetDistanceToPlayer(this.GlobalPosition) >= 20f)
            return;

        GameMenu.instance.OnOpenIslandTab();
        IslandMenu.instance.current_sign = this;

        Debug.Print(GameManager.island_matrix.ToString());
    }
}
