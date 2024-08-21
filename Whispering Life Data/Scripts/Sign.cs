using System;
using Godot;

public partial class Sign : Building_Node
{
    [Export]
    public Island_Properties current_ip;

    [Export]
    public Island_Properties.DIRECTION dir;

    public override void OnMouseClick()
    {
        if (Global.GetDistanceToPlayer(this.GlobalPosition) >= 20f)
            return;

        island_menu.instance.current_sign = this;
        island_menu.instance.SwitchBuildingMenu();
    }
}
