using System.Diagnostics;
using Godot;

public partial class Giver : Area2D
{
    [Export]
    public Detector detector;

    [Export]
    public placeable_building building;

    [Export]
    public Node2D Holder;
    public PackedScene beltItem = ResourceLoader.Load<PackedScene>("res://belt_item.tscn");

    [Export]
    public Item produced_item;

    [Export]
    public Timer crafting_timer;

    public void OnProductionTimerTimeout()
    {
        detector.Detect();
    }

    private bool is_crafting = false;

    public void OnDetectorBeltDetected(Belt destination)
    {
        if (building.is_crafter)
        {
            if (destination.can_receive_item() && building.export_count > 0)
            {
                building.export_count--;
                BeltItem item = (BeltItem)beltItem.Instantiate();
                item.InitBeltItem(produced_item);
                Holder.AddChild(item);
                destination.receive_item(item);
            }
            if (!is_crafting && building.count >= 2)
            {
                building.count -= 2;
                is_crafting = true;
                crafting_timer.OneShot = true;
                crafting_timer.Start();
            }
        }
        else
        {
            if (destination.can_receive_item() && building.count > 0)
            {
                building.count -= 1;
                BeltItem item = (BeltItem)beltItem.Instantiate();
                Holder.AddChild(item);
                destination.receive_item(item);
            }
        }
    }

    public void OnCraftingTimerTimeout()
    {
        building.export_count++;
        is_crafting = false;
    }
}
