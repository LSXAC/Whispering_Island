using System;
using System.Diagnostics;
using Godot;

public partial class Sign : Building_Node
{
    [Export]
    public Island_Properties current_ip;

    [Export]
    public Island_Properties.DIRECTION dir;

    public override void OnMouseClick()
    {
        if (GlobalFunctions.GetDistanceToPlayer(this.GlobalPosition) >= 20f)
            return;
        GameMenu.INSTANCE.OnOpenIslandTab();
        island_menu.instance.current_sign = this;

        Debug.Print(Game_Manager.island_matrix.ToString());
    }
}
