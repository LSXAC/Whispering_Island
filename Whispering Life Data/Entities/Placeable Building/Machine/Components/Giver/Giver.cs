using System.Diagnostics;
using Godot;

public partial class Giver : Area2D
{
    [Export]
    public Detector detector;

    [Export]
    public TransportBase.Direction direction_not_giving;

    [Export]
    public TransportBase.Direction to_direction;

    [Export]
    public MachineBase building;

    [Export]
    public PathConnectArea path_connect_area;

    [Export]
    public Node2D Holder;
    public PackedScene beltItem = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://dkue7sa7xyeyr")
    );

    public override void _Ready()
    {
        switch (to_direction)
        {
            case TransportBase.Direction.Top:
                path_connect_area.Position = new Vector2(
                    path_connect_area.Position.X,
                    path_connect_area.Position.Y + 12
                );
                break;
            case TransportBase.Direction.Right:
                path_connect_area.Position = new Vector2(
                    path_connect_area.Position.X - 12,
                    path_connect_area.Position.Y
                );
                break;
            case TransportBase.Direction.Down:
                path_connect_area.Position = new Vector2(
                    path_connect_area.Position.X,
                    path_connect_area.Position.Y - 12
                );
                break;
            case TransportBase.Direction.Left:
                path_connect_area.Position = new Vector2(
                    path_connect_area.Position.X + 12,
                    path_connect_area.Position.Y
                );
                break;
        }
    }

    public void OnProductionTimerTimeout()
    {
        detector.Detect();
    }

    public void DisableMonitoring()
    {
        detector.Monitoring = false;
        if (path_connect_area == null)
        {
            Debug.Print(
                "No PathConnectArea referenced! Belt connections will not be disabled",
                this
            );
            return;
        }
        path_connect_area.DisableArea();
    }

    public void EnableMonitoring()
    {
        detector.Monitoring = true;
        if (path_connect_area == null)
        {
            Debug.Print(
                "No PathConnectArea referenced! Belt connections will not be enabled",
                this
            );
            return;
        }
        path_connect_area.EnableArea();
    }

    public void OnDetectorBeltDetected(Area2D destination)
    {
        if (destination.GetParent() is Belt belt)
        {
            Debug.Print(
                "Belt to Direction: "
                    + belt.to_direction.ToString()
                    + " | "
                    + " Direction Not Giving: "
                    + direction_not_giving.ToString()
            );
            if (belt.to_direction == direction_not_giving)
                return;

            if (!building.machine_enabled)
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
        if (process_building.item_array[(int)ProcessingTab.SlotType.EXPORT] != null)
            if (process_building.item_array[(int)ProcessingTab.SlotType.EXPORT].amount > 0)
            {
                process_building.item_array[(int)ProcessingTab.SlotType.EXPORT].amount--;
                AddBeltItemToBelt(
                    belt,
                    new Item(process_building.GetItemResource(ProcessingTab.SlotType.EXPORT), 1)
                );

                if (process_building.item_array[(int)ProcessingTab.SlotType.EXPORT].amount == 0)
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
            ChestInventory.instance.RemoveItem(item, ((ChestBase)building).chest_items);

            AddBeltItemToBelt(
                belt,
                new Item(Inventory.ITEM_TYPES[(Inventory.ITEM_ID)i_s.item_id], 1)
            );
            ChestInventory.instance.UpdateInventoryUI();
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
            ProcessingTab.instance.UpdateUI();
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
