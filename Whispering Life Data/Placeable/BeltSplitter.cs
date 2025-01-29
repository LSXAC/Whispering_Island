using System;
using Godot;

public partial class BeltSplitter : Belt
{
    [Export]
    public Timer checkAreaTimer;

    [Export]
    public Belt belt_0,
        belt_1,
        belt_2;

    int row = 0;

    public new bool can_receive_item()
    {
        if (item_holder == null)
            return false;

        return item_holder.GetChildCount() == 0;
    }

    public void CheckTimerTimeout()
    {
        if (item_holder.GetChildCount() == 0)
            return;

        if (item_holder.GetChildCount() > 0)
            if (item_holder.moving_item)
                return;

        if (belt_2 == null)
        {
            if (row == 1)
                row = 0;
            else
                row = 1;
        }
        else
        {
            if (row == 0)
                row = 1;
            else if (row == 1)
                row = 2;
            else
                row = 0;
        }

        if (row == 0)
            if (belt_0.item_holder.GetChildCount() == 0)
            {
                var item = item_holder.offload_item();
                belt_0.item_holder.GetParent<Belt>().receive_item(item);
                return;
            }

        if (row == 1)
            if (belt_1.item_holder.GetChildCount() == 0)
            {
                var item = item_holder.offload_item();
                belt_1.item_holder.GetParent<Belt>().receive_item(item);
                return;
            }

        if (row == 2)
            if (belt_2.item_holder.GetChildCount() == 0)
            {
                var item = item_holder.offload_item();
                belt_2.item_holder.GetParent<Belt>().receive_item(item);
                return;
            }
    }
}
