using Godot;
using Godot.Collections;

public partial class Slot : Button
{
    [Export]
    public string label_translation_string;

    [Export]
    public ItemInfo.Type slot_type;

    [Export]
    public bool is_export_slot = false;

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
        if (
            slot_type == ItemInfo.Type.CLOTHS
            || slot_type == ItemInfo.Type.TOOL
            || slot_type == ItemInfo.Type.HEAD
            || slot_type == ItemInfo.Type.CHESTPLATE
            || slot_type == ItemInfo.Type.LEGGINGS
            || slot_type == ItemInfo.Type.SHOES
        )
        {
            Pressed += () => OnEquipSlotButton(GetIndex());
        }
        if (slot_type == ItemInfo.Type.RESEARCHABLE)
        {
            Pressed += () => OnResearchSlotButton();
        }
    }

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        if (label_translation_string == null)
            return;

        Text = TranslationServer.Translate(label_translation_string);
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
                if (GetItem().item_info.HasType(ItemInfo.Type.TOOL))
                {
                    EquipmentPanel.INSTANCE.equipped_tools[index] = null;
                    EquipmentPanel.INSTANCE.slots_tool[index].ClearItem();
                }
                if (
                    GetItem().item_info.HasType(ItemInfo.Type.HEAD)
                    || GetItem().item_info.HasType(ItemInfo.Type.CHESTPLATE)
                    || GetItem().item_info.HasType(ItemInfo.Type.LEGGINGS)
                    || GetItem().item_info.HasType(ItemInfo.Type.SHOES)
                )
                {
                    EquipmentPanel.INSTANCE.equipped_armor[index] = null;
                    EquipmentPanel.INSTANCE.slots_armor[index].ClearItem();
                }
                EquipmentPanel.INSTANCE.CalculateStatsFromEquipment();
            }
        }
        else
        {
            if (GetItem() == null)
            {
                if (
                    Inventory.clicked_item.item_info.HasType(slot_type)
                    && (
                        slot_type == ItemInfo.Type.HEAD
                        || slot_type == ItemInfo.Type.CHESTPLATE
                        || slot_type == ItemInfo.Type.LEGGINGS
                        || slot_type == ItemInfo.Type.SHOES
                    )
                )
                {
                    OnTakeItemFromArmorSlot(index);
                    return;
                }

                if (
                    Inventory.clicked_item.item_info.HasType(slot_type)
                    && slot_type == ItemInfo.Type.TOOL
                )
                {
                    EquipmentPanel.INSTANCE.equipped_tools[index] = new ItemSave(
                        (int)Inventory.clicked_item.item_info.unique_id,
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

    private void OnTakeItemFromArmorSlot(int index)
    {
        EquipmentPanel.INSTANCE.equipped_armor[index] = new ItemSave(
            (int)Inventory.clicked_item.item_info.unique_id,
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

    public void OnSlotButton()
    {
        if (Inventory.clicked_item == null)
        {
            if (GetItem() != null)
            {
                CreateClickedItem();
                inventory_base.inventory_items[GetIndex()] = null;
                inventory_base.UpdateSlotUI(GetIndex());
            }
        }
        else
        {
            if (GetItem() == null)
            {
                inventory_base.inventory_items[GetIndex()] = new ItemSave(
                    (int)Inventory.clicked_item.item_info.unique_id,
                    Inventory.clicked_item.amount
                );
                inventory_base.UpdateSlotUI(GetIndex(), Inventory.clicked_item);
                Inventory.clicked_item.QueueFree();
                Inventory.clicked_item = null;
                return;
            }
            if (GetItem() != null)
                if (GetItem().item_info == Inventory.clicked_item.item_info)
                {
                    inventory_base.inventory_items[GetIndex()].amount += Inventory
                        .clicked_item
                        .amount;
                    inventory_base.UpdateSlotUI(GetIndex(), Inventory.clicked_item);
                    Inventory.clicked_item.QueueFree();
                    Inventory.clicked_item = null;
                    return;
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
                    (int)Inventory.clicked_item.item_info.unique_id,
                    Inventory.clicked_item.amount
                );
                inventory_base.UpdateInventoryUI();
                Inventory.clicked_item.QueueFree();
                Inventory.clicked_item = null;
                return;
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
            return;
        }
    }

    public void OnResearchSlotButton()
    {
        if (Inventory.clicked_item == null)
        {
            if (GetItem() != null)
            {
                CreateClickedItem();
                ResearchTab.research_slot_item = null;
                ResearchTab.INSTANCE.UpdateLevelTabs();
            }
        }
        else
        {
            if (!Inventory.clicked_item.item_info.HasType(ItemInfo.Type.RESEARCHABLE))
                return;
            if (GetItem() == null)
            {
                ResearchTab.research_slot_item = new ItemSave(
                    (int)Inventory.clicked_item.item_info.unique_id,
                    Inventory.clicked_item.amount
                );
                Inventory.clicked_item.QueueFree();
                Inventory.clicked_item = null;
                ResearchTab.INSTANCE.UpdateLevelTabs();
                return;
            }
        }
    }

    public void CreateClickedItem()
    {
        InventoryItem ii = new InventoryItem();
        ii.init(GetItem().item_info);
        ii.amount = GetItem().amount;
        ii.MouseFilter = MouseFilterEnum.Ignore;
        ii.ZIndex = 10;
        ii.SelfModulate = GetItem().SelfModulate;
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
                if (Inventory.clicked_item.item_info.HasType(slot_type))
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
                            FurnaceTab.INSTANCE.UpdateFurnaceUI();
                            Inventory.clicked_item.QueueFree();
                            Inventory.clicked_item = null;
                            break;

                        /* EXPORT SLOT should not Items be placed, only taken out
                        case (int)FurnaceTab.SlotType.EXPORT:
                            FurnaceTab.INSTANCE.process_building.export_item_info = Inventory
                                .clicked_item
                                .item_info;
                            FurnaceTab.INSTANCE.process_building.export_count = Inventory
                                .clicked_item
                                .amount;
                            break;*/

                        case (int)FurnaceTab.SlotType.FUEL:
                            FurnaceTab.INSTANCE.process_building.fuel_item_info = Inventory
                                .clicked_item
                                .item_info;
                            FurnaceTab.INSTANCE.process_building.fuel_count = Inventory
                                .clicked_item
                                .amount;
                            FurnaceTab.INSTANCE.UpdateFurnaceUI();
                            Inventory.clicked_item.QueueFree();
                            Inventory.clicked_item = null;
                            break;
                    }
                }
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
