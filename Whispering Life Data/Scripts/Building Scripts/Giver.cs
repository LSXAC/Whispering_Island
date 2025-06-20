using System.Diagnostics;
using Godot;

public partial class Giver : Area2D
{
    [Export]
    public Detector detector;

    [Export]
    public Belt.Direction direction_not_giving;

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
            else if (building is ProductionMachine && building is not ChestBase)
                IsProductionBuilding(destination);
            else if (building is ChestBase || building is RailStation)
                IsChestBuilding(destination);
        }
    }

    private void IsProcessingBuilding(Node destination)
    {
        if (((ProcessBuilding)building).item_array[(int)FurnaceTab.SlotType.EXPORT] != null)
            if (
                destination.GetParent<Belt>().can_receive_item()
                && ((ProcessBuilding)building).item_array[(int)FurnaceTab.SlotType.EXPORT].amount
                    > 0
            )
            {
                ((ProcessBuilding)building).item_array[(int)FurnaceTab.SlotType.EXPORT].amount--;
                BeltItem item = (BeltItem)beltItem.Instantiate();

                item.Init(
                    new Item(
                        ((ProcessBuilding)building).GetItemResource(FurnaceTab.SlotType.EXPORT),
                        1
                    )
                );
                Holder.AddChild(item);
                destination.GetParent<Belt>().receive_item(item);

                if (destination.GetParent() is BeltTunnel)
                {
                    destination.GetParent<BeltTunnel>().from_Belt = true;
                    destination.GetParent<BeltTunnel>().GetNode<Timer>("CheckTimer").Start();
                }
            }
    }

    private void IsChestBuilding(Node destination)
    {
        if (destination.GetParent<Belt>().can_receive_item())
        {
            if (destination.GetParent() is BeltTunnel)
                destination.GetParent<BeltTunnel>().from_Belt = true;

            if (destination.GetParent() is BeltSplitter)
                destination.GetParent<BeltSplitter>().GetNode<Timer>("CheckAreaTimer").Start();

            if (destination.GetParent() is BeltCombiner)
                destination.GetParent<BeltCombiner>().GetNode<Timer>("CheckAreaTimer").Start();

            //Remove from Chest
            foreach (ItemSave i_s in ((ChestBase)building).chest_items)
            {
                if (i_s == null)
                    continue;

                Item item = new Item(Inventory.ITEM_TYPES[(Inventory.ITEM_ID)i_s.item_id], 1);
                ChestInventoryUI.instance.RemoveItem(item, ((ChestBase)building).chest_items);

                BeltItem belt_item = (BeltItem)beltItem.Instantiate();
                belt_item.Init(new Item(Inventory.ITEM_TYPES[(Inventory.ITEM_ID)i_s.item_id], 1));
                Holder.AddChild(belt_item);
                destination.GetParent<Belt>().receive_item(belt_item);
                ChestInventoryUI.instance.UpdateInventoryUI();
                break;
            }
        }
    }

    private void IsProductionBuilding(Node destination)
    {
        if (
            destination.GetParent<Belt>().can_receive_item()
            && ((ProductionMachine)building).count > 0
        )
        {
            if (destination.GetParent() is BeltTunnel)
                destination.GetParent<BeltTunnel>().from_Belt = true;

            if (destination.GetParent() is BeltSplitter)
                destination.GetParent<BeltSplitter>().GetNode<Timer>("CheckAreaTimer").Start();

            if (destination.GetParent() is BeltCombiner)
                destination.GetParent<BeltCombiner>().GetNode<Timer>("CheckAreaTimer").Start();

            ((ProductionMachine)building).count -= 1;
            BeltItem item = (BeltItem)beltItem.Instantiate();
            item.Init(new Item(((ProductionMachine)building).output_item_resource, 1));
            Holder.AddChild(item);
            destination.GetParent<Belt>().receive_item(item);
            FurnaceTab.instance.UpdateFurnaceUI();
        }
    }
}
