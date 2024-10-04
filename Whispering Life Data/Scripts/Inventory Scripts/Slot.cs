using System.Diagnostics;
using System.Xml.Serialization;
using Godot;

public partial class Slot : Button
{
    [Export]
    public ItemInfo.Type type;

    InventoryBase inventory_base = null;

    public override void _Ready()
    {
        if (GetParent().GetParent() is Inventory)
        {
            inventory_base = (InventoryBase)GetParent().GetParent();
            Pressed += () => OnSlotButton();
        }
        if (GetParent().GetParent() is ChestInventory)
        {
            inventory_base = (InventoryBase)GetParent().GetParent();
            Pressed += () => OnChestSlotButton();
        }
        if (type == ItemInfo.Type.Cloths || type == ItemInfo.Type.Tool)
        {
            Pressed += () => OnEquipSlotButton(GetIndex());
        }
    }

    public void SetItem(ItemInfo item_info, int amount)
    {
        InventoryItem ii = new InventoryItem();
        ii.init(item_info);
        ii.amount = amount;
        ii.UpdateAmountLabel();
        AddChild(ii);
        if (amount == 0)
            ClearItem();
    }

    public void UpdateItem(int amount)
    {
        GetItem().amount += amount;
        GetItem().UpdateAmountLabel();
        if (GetItem().amount == 0)
            ClearItem();
    }

    public void UpdateFurnaceItem(ItemInfo item_info, int amount)
    {
        if (GetItem() == null)
            SetItem(item_info, amount);
        GetItem().amount += amount;
        GetItem().UpdateAmountLabel();
        if (GetItem().amount <= 0)
            ClearItem();
    }

    public InventoryItem GetItem()
    {
        if (GetChildCount() > 0)
            return (InventoryItem)GetChild(0);
        return null;
    }

    public void ClearItem()
    {
        if (GetItem() == null)
            return;
        GetItem().QueueFree();
    }

    public void OnEquipSlotButton(int index)
    {
        if (Inventory.clicked_item == null)
        {
            if (GetItem() != null)
            {
                CreateClickedItem();
                if (GetItem().item_info.item_type == ItemInfo.Type.Cloths)
                {
                    EquipmentPanel.INSTANCE.equipped_armor[index] = null;
                    EquipmentPanel.INSTANCE.slots_armor[index].ClearItem();
                }
                if (GetItem().item_info.item_type == ItemInfo.Type.Tool)
                {
                    EquipmentPanel.INSTANCE.equipped_tools[index] = null;
                    EquipmentPanel.INSTANCE.slots_tool[index].ClearItem();
                }
                EquipmentPanel.INSTANCE.CalculateStatsFromEquipment();
            }
        }
        else
        {
            if (GetItem() == null)
            {
                if (
                    type == Inventory.clicked_item.item_info.item_type
                    && type == ItemInfo.Type.Cloths
                )
                {
                    EquipmentPanel.INSTANCE.equipped_armor[index] = new ItemSave(
                        Inventory.clicked_item.item_info.unique_item_id,
                        Inventory.clicked_item.amount
                    );
                    EquipmentPanel
                        .INSTANCE.slots_armor[index]
                        .SetItem(Inventory.clicked_item.item_info, Inventory.clicked_item.amount);
                    //inventory_base.UpdateInventoryUI();
                    Inventory.clicked_item.QueueFree();
                    Inventory.clicked_item = null;
                    EquipmentPanel.INSTANCE.CalculateStatsFromEquipment();
                }
                if (
                    type == Inventory.clicked_item.item_info.item_type
                    && type == ItemInfo.Type.Tool
                )
                {
                    EquipmentPanel.INSTANCE.equipped_tools[index] = new ItemSave(
                        Inventory.clicked_item.item_info.unique_item_id,
                        Inventory.clicked_item.amount
                    );
                    EquipmentPanel
                        .INSTANCE.slots_tool[index]
                        .SetItem(Inventory.clicked_item.item_info, Inventory.clicked_item.amount);
                    //inventory_base.UpdateInventoryUI();
                    Inventory.clicked_item.QueueFree();
                    Inventory.clicked_item = null;
                    EquipmentPanel.INSTANCE.CalculateStatsFromEquipment();
                }
                //Update Notification to EquipmentPanel to update Stats (Bonus)
            }
            else // Can be Expanded, if e.g. Stackable Potions in one Equipment Slot
                return;
        }
    }

    public void OnSlotButton()
    {
        if (Inventory.clicked_item == null)
        {
            if (GetItem() != null)
            {
                CreateClickedItem();
                inventory_base.inventory_items[GetIndex()] = null;
                inventory_base.UpdateInventoryUI();
            }
        }
        else
        {
            if (GetItem() == null)
            {
                inventory_base.inventory_items[GetIndex()] = new ItemSave(
                    Inventory.clicked_item.item_info.unique_item_id,
                    Inventory.clicked_item.amount
                );
                inventory_base.UpdateInventoryUI();
                Inventory.clicked_item.QueueFree();
                Inventory.clicked_item = null;
            }
            if (GetItem() != null)
                if (GetItem().item_info ==
                 Inventory.clicked_item.item_info)
                {
                    inventory_base.inventory_items[GetIndex()].amount += Inventory
                        .clicked_item
                        .amount;
                    inventory_base.UpdateInventoryUI();
                    Inventory.clicked_item.QueueFree();
                    Inventory.clicked_item = null;
                }
        }
    }

    public void OnChestSlotButton()
    {
        if (Inventory.clicked_item == null)
        {
            if (GetItem() != null)
            {
                CreateClickedItem();
                ChestInventory.INSTANCE.current_chest.chest_items[GetIndex()] = null;
                inventory_base.UpdateInventoryUI();
            }
        }
        else
        {
            if (GetItem() == null)
            {
                ChestInventory.INSTANCE.current_chest.chest_items[GetIndex()] = new ItemSave(
                    Inventory.clicked_item.item_info.unique_item_id,
                    Inventory.clicked_item.amount
                );
            }
            if (GetItem() != null)
                if (GetItem().item_info == Inventory.clicked_item.item_info)
                {
                    ChestInventory.INSTANCE.current_chest.chest_items[GetIndex()].amount +=
                        Inventory.clicked_item.amount;
                }
            inventory_base.UpdateInventoryUI();
            Inventory.clicked_item.QueueFree();
            Inventory.clicked_item = null;
        }
    }

    public void CreateClickedItem()
    {
        InventoryItem ii = new InventoryItem();
        ii.init(GetItem().item_info);
        ii.amount = GetItem().amount;
        ii.MouseFilter = MouseFilterEnum.Ignore;
        ii.ZIndex = 10;
        Inventory.clicked_item = ii;
        Inventory.INSTANCE.AddChild(ii);
    }

    public void onMachineSlot(int id)
    {
        if (Inventory.clicked_item == null)
        {
            if (GetItem() != null)
                switch (id)
                {
                    case (int)FurnaceTab.SlotType.IMPORT:
                        CreateClickedItem();
                        FurnaceTab.INSTANCE.process_building.import_item_info = null;
                        FurnaceTab.INSTANCE.process_building.import_count = 0;
                        ClearItem();
                        break;
                    case (int)FurnaceTab.SlotType.EXPORT:
                        CreateClickedItem();
                        FurnaceTab.INSTANCE.process_building.export_item_info = null;
                        FurnaceTab.INSTANCE.process_building.export_count = 0;
                        ClearItem();
                        break;
                    case (int)FurnaceTab.SlotType.FUEL:
                        CreateClickedItem();
                        FurnaceTab.INSTANCE.process_building.fuel_item_info = null;
                        FurnaceTab.INSTANCE.process_building.fuel_count = 0;
                        ClearItem();
                        break;
                }
        }
        else
        {
            if (GetItem() == null)
            {
                switch (id)
                {
                    case (int)FurnaceTab.SlotType.IMPORT:
                        FurnaceTab.INSTANCE.process_building.import_item_info = Inventory
                            .clicked_item
                            .item_info;
                        FurnaceTab.INSTANCE.process_building.import_count = Inventory
                            .clicked_item
                            .amount;
                        break;

                    case (int)FurnaceTab.SlotType.EXPORT:
                        FurnaceTab.INSTANCE.process_building.export_item_info = Inventory
                            .clicked_item
                            .item_info;
                        FurnaceTab.INSTANCE.process_building.export_count = Inventory
                            .clicked_item
                            .amount;
                        break;

                    case (int)FurnaceTab.SlotType.FUEL:
                        FurnaceTab.INSTANCE.process_building.fuel_item_info = Inventory
                            .clicked_item
                            .item_info;
                        FurnaceTab.INSTANCE.process_building.fuel_count = Inventory
                            .clicked_item
                            .amount;
                        break;
                }
                FurnaceTab.INSTANCE.UpdateFurnaceUI();
                Inventory.clicked_item.QueueFree();
                Inventory.clicked_item = null;
            }
        }
    }

    /*public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (data.Obj == null)
            return false;

        InventoryItem ii = (InventoryItem)data;
        if (type == ItemInfo.Type.Resource)
        {
            if (GetChildCount() == 0)
                return true;
            else if (type == ii.GetParent<Slot>().type)
                return true;
        }
        else
        {
            return ii.item_info.item_type == type;
        }

        return false;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (data.Obj == null)
            return;

        InventoryItem ii = (InventoryItem)data;
        if (GetChildCount() > 0)
        {
            InventoryItem item = GetChild<InventoryItem>(0);
            if ((Node)data == item)
                return;

            if (item.item_info == ii.item_info)
            {
                item.amount += ii.amount;
                item.UpdateAmountLabel();
                ii.QueueFree();
                return;
            }
            item.Reparent(((Node)data).GetParent());
        }
        ((Node)data).Reparent(this);
    }*/
}
