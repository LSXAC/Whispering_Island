using System;
using System.Diagnostics;
using Godot;

public partial class Rail : TransportBase
{
    public void OnDetectorRailDetected(Area2D area)
    {
        if (ignore_self_detector)
            return;

        Debug.Print("Rail: " + area.Name);
        if (area is PathConnectArea)
        {
            if (area.GetParent() is Rail)
            {
                if (area.GetParent<TransportBase>().can_receive_item())
                {
                    var item = item_holder.offload_item();
                    area.GetParent<Rail>().receive_item(item);
                }
            }
        }
    }

    public void OnItemHolderItemHeld()
    {
        detector.Detect();
    }
}
