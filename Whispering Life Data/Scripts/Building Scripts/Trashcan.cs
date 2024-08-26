using System;
using Godot;

public partial class Trashcan : Area2D
{
    [Export]
    public ItemHolder item_holder;

    public bool can_receive_item()
    {
        return item_holder.GetChildCount() == 0;
    }

    public void receive_item(Node2D item)
    {
        item_holder.receive_item(item);
    }

    public void OnItemHolderItemHeld()
    {
        var item = item_holder.offload_item();
        item.QueueFree();
    }
}
