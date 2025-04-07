using System;
using Godot;

public partial class Minecart : MoveableBase
{
    public bool is_running = false;

    public override void OnMouseClick()
    {
        MinecartTab.current_minecart = this;
        GameMenu.INSTANCE.OnOpenMinecartTab();
    }
}
