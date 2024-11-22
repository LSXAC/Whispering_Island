using System.Diagnostics;
using Godot;

public partial class Giver : Area2D
{
    [Export]
    public Detector detector;

    [Export]
    public Belt.BeltDirection direction_not_giving;

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
        //when Destination is Belt
        if (destination.GetParent() is Belt)
        {
            if (((Belt)destination.GetParent()).to_direction == direction_not_giving)
                return;

            if (building is ProcessBuilding)
                IsProcessingBuilding(destination);
            else if (building is ProductionMachine)
                IsProductionBuilding(destination);
        }
    }

    private void IsProcessingBuilding(Node destination)
    {
        if (
            destination.GetParent<Belt>().can_receive_item()
            && ((ProcessBuilding)building).export_count > 0
        )
        {
            ((ProcessBuilding)building).export_count--;
            BeltItem item = (BeltItem)beltItem.Instantiate();
            item.InitBeltItem(new Item(((ProcessBuilding)building).export_item_info, 1));
            Holder.AddChild(item);
            destination.GetParent<Belt>().receive_item(item);
            FurnaceTab.INSTANCE.UpdateFurnaceUI();
        }
    }

    private void IsProductionBuilding(Node destination)
    {
        if (
            destination.GetParent<Belt>().can_receive_item()
            && ((ProductionMachine)building).production_count > 0
        )
        {
            ((ProductionMachine)building).production_count -= 1;
            BeltItem item = (BeltItem)beltItem.Instantiate();
            item.InitBeltItem(new Item(((ProductionMachine)building).production_item_info, 1));
            Holder.AddChild(item);
            destination.GetParent<Belt>().receive_item(item);
            FurnaceTab.INSTANCE.UpdateFurnaceUI();
        }
    }
}
