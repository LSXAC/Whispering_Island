using System.Diagnostics;
using Godot;

public partial class Giver : Area2D
{
    [Export]
    public Detector detector;

    [Export]
    public ProcessBase building;

    [Export]
    public Node2D Holder;
    public PackedScene beltItem = ResourceLoader.Load<PackedScene>("res://belt_item.tscn");

    [Export]
    public Item produced_item;

    public void OnProductionTimerTimeout()
    {
        detector.Detect();
    }

    public void OnDetectorBeltDetected(Belt destination)
    {
        if (building is ProcessBuilding)
        {
            if (destination.can_receive_item() && building.export_count > 0)
            {
                building.export_count--;
                BeltItem item = (BeltItem)beltItem.Instantiate();
                item.InitBeltItem(produced_item);
                Holder.AddChild(item);
                destination.receive_item(item);
            }
            if (!((ProcessBuilding)building).is_crafting && building.input_count >= 2)
            {
                building.input_count -= 2;
                ((ProcessBuilding)building).is_crafting = true;
                ((ProcessBuilding)building).crafting_timer.OneShot = true;
                ((ProcessBuilding)building).crafting_timer.Start();
            }
        }
        else if (building is ProductionMachine)
        {
            if (destination.can_receive_item() && building.input_count > 0)
            {
                building.input_count -= 1;
                BeltItem item = (BeltItem)beltItem.Instantiate();
                Holder.AddChild(item);
                destination.receive_item(item);
            }
        }
    }
}
