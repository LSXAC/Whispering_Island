using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class RemoveSign : Sign
{
    [Export]
    public RemovableObjectsManager removable_objects_Manager;

    [Export]
    public Node2D object_connected;

    [Export]
    public Array<Item> required_items;

    [Export]
    public int id = 0;

    public override void OnMouseClick()
    {
        if (GlobalFunctions.GetDistanceToPlayer(this.GlobalPosition) >= 20f)
            return;

        if (required_items == null || required_items?.Count == 0)
        {
            Debug.Print("No items required to remove this sign.");
            return;
        }

        DestroyMenu.current_sign = this;
        GameMenu.instance.OnOpenDestroyTab();
    }
}
