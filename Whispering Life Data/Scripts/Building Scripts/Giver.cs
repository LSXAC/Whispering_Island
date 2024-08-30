using System.Diagnostics;
using Godot;

public partial class Giver : Area2D
{
    [Export]
    public Detector detector;

    [Export]
    public MachineBase building;

    [Export]
    public Node2D Holder;
    public PackedScene beltItem = ResourceLoader.Load<PackedScene>("res://belt_item.tscn");

    public void OnProductionTimerTimeout()
    {
        detector.Detect();
    }

    public void OnDetectorBeltDetected(Area2D destination)
    {
        if (destination.GetParent() is Belt)
        {
            if (building is ProcessBuilding)
            {
                if (destination.GetParent<Belt>().can_receive_item() && building.export_count > 0)
                {
                    building.export_count--;
                    FurnaceTab.INSTANCE.OnVisiblityChange();
                    BeltItem item = (BeltItem)beltItem.Instantiate();
                    item.InitBeltItem(new Item(building.export_item_info, 1));
                    Holder.AddChild(item);
                    destination.GetParent<Belt>().receive_item(item);
                }
            }
            else if (building is ProductionMachine)
            {
                if (destination.GetParent<Belt>().can_receive_item() && building.import_count > 0)
                {
                    building.import_count -= 1;
                    BeltItem item = (BeltItem)beltItem.Instantiate();
                    item.InitBeltItem(new Item(building.export_item_info, 1));
                    Holder.AddChild(item);
                    destination.GetParent<Belt>().receive_item(item);
                }
            }
        }
    }
}
