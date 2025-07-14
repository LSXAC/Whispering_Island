using System.Diagnostics;
using Godot;

public partial class Giver : Area2D
{
    [Export]
    public Detector detector;

    [Export]
    public TransportBase.Direction direction_not_giving;

    [Export]
    public MachineBase building;

    [Export]
    public Node2D Holder;
    public PackedScene beltItem = ResourceLoader.Load<PackedScene>(
        "res://Scenes/Items/belt_item.tscn"
    );

    public void OnProductionTimerTimeout()
    {
        detector.Detect();
    }

    public void DisableMonitoring()
    {
        detector.Monitoring = false;
    }

    public void EnableMonitoring()
    {
        detector.Monitoring = true;
    }

    public void OnDetectorBeltDetected(Area2D destination)
    {
        if (destination.GetParent() is Belt belt)
        {
            Debug.Print(belt.to_direction.ToString() + " | " + direction_not_giving.ToString());
            if (belt.to_direction == direction_not_giving)
                return;

            if (building is ProcessBuilding)
                IsProcessingBuilding(belt);
            else if (building is ProductionMachine && building is not ChestBase)
                IsProductionBuilding(belt);
            else if (building is ChestBase || building is RailStation)
                IsChestBuilding(belt);
        }
    }

    private void IsProcessingBuilding(Belt belt)
    {
        ProcessBuilding process_building = (ProcessBuilding)building;
        if (process_building.item_array[(int)FurnaceTab.SlotType.EXPORT] != null)
            if (process_building.item_array[(int)FurnaceTab.SlotType.EXPORT].amount > 0)
            {
                process_building.item_array[(int)FurnaceTab.SlotType.EXPORT].amount--;
                AddBeltItemToBelt(
                    belt,
                    new Item(process_building.GetItemResource(FurnaceTab.SlotType.EXPORT), 1)
                );

                if (process_building.item_array[(int)FurnaceTab.SlotType.EXPORT].amount == 0)
                    process_building.ResetExportSlot();

                if (belt is BeltTunnel belt_tunnel)
                {
                    belt_tunnel.from_Belt = true;
                    belt_tunnel.GetNode<Timer>("CheckTimer").Start();
                }
            }
    }

    private void IsChestBuilding(Belt belt)
    {
        if (!belt.can_receive_item())
            return;

        if (belt is BeltTunnel belt_tunnel)
            belt_tunnel.from_Belt = true;

        if (belt is BeltSplitter belt_splitter)
            belt_splitter.GetNode<Timer>("CheckAreaTimer").Start();

        if (belt is BeltCombiner belt_combiner)
            belt_combiner.GetNode<Timer>("CheckAreaTimer").Start();

        foreach (ItemSave i_s in ((ChestBase)building).chest_items)
        {
            if (i_s == null)
                continue;

            Item item = new Item(Inventory.ITEM_TYPES[(Inventory.ITEM_ID)i_s.item_id], 1);
            ChestInventoryUI.instance.RemoveItem(item, ((ChestBase)building).chest_items);

            AddBeltItemToBelt(
                belt,
                new Item(Inventory.ITEM_TYPES[(Inventory.ITEM_ID)i_s.item_id], 1)
            );
            ChestInventoryUI.instance.UpdateInventoryUI();
            break;
        }
    }

    private void IsProductionBuilding(Belt belt)
    {
        if (belt.can_receive_item() && ((ProductionMachine)building).count > 0)
        {
            if (belt is BeltTunnel belt_tunnel)
                belt_tunnel.from_Belt = true;

            if (belt is BeltSplitter belt_splitter)
                belt_splitter.GetNode<Timer>("CheckAreaTimer").Start();

            if (belt is BeltCombiner belt_combiner)
                belt_combiner.GetNode<Timer>("CheckAreaTimer").Start();

            ((ProductionMachine)building).count -= 1;
            AddBeltItemToBelt(
                belt,
                new Item(((ProductionMachine)building).output_item_resource, 1)
            );
            FurnaceTab.instance.UpdateFurnaceUI();
        }
    }

    private void AddBeltItemToBelt(Belt belt, Item item)
    {
        BeltItem belt_item = (BeltItem)beltItem.Instantiate();
        belt_item.Init(item);
        Holder.AddChild(belt_item);
        belt.receive_item(belt_item);
    }
}
