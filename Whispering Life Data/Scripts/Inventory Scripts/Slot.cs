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
    Chest chest = null;

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton btn && @event.IsPressed())
        {
            if (GetParent().GetParent() is Inventory)
            {
                inventory_base = (InventoryBase)GetParent().GetParent();
                OnSlotButton(btn);
            }
            if (GetParent().GetParent() is ChestInventory)
            {
                inventory_base = (InventoryBase)GetParent().GetParent();
                chest = ChestInventory.INSTANCE.current_chest;
                OnChestSlotButton(btn);
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
                OnEquipSlotButton(GetIndex());
            }
            if (slot_type == ItemInfo.Type.RESEARCHABLE)
            {
                OnResearchSlotButton();
            }
            AcceptEvent();
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
                    ClearClickedItem();
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
        ClearClickedItem();
        EquipmentPanel.INSTANCE.CalculateStatsFromEquipment();
    }

    public void OnSlotButton(InputEventMouseButton @btn)
    {
        if (Inventory.clicked_item == null)
        {
            if (GetItem() != null)
            {
                if (ItemCanBeHalfed(GetItem()))
                    if (@btn.ButtonMask == MouseButtonMask.Right)
                    {
                        CreateClickedItem(halfNext: true);
                        UpdateSlot(
                            inventory_base.inventory_items,
                            HalfAmount(inventory_base.inventory_items[GetIndex()].amount)
                        );
                        return;
                    }

                CreateClickedItem();
                ClearSlot(inventory_base.inventory_items);
            }
            return;
        }
        else
        {
            if (GetItem() == null)
            {
                if (ItemCanBeHalfed(Inventory.clicked_item))
                    if (btn.ButtonMask == MouseButtonMask.Right)
                    {
                        NewSlot(
                            inventory_base.inventory_items,
                            (int)Inventory.clicked_item.item_info.unique_id,
                            HalfAmountNextInt(Inventory.clicked_item.amount)
                        );
                        Inventory.clicked_item.amount = (int)(Inventory.clicked_item.amount / 2.0);
                        return;
                    }

                NewSlot(
                    inventory_base.inventory_items,
                    (int)Inventory.clicked_item.item_info.unique_id,
                    Inventory.clicked_item.amount
                );
                ClearClickedItem();
                return;
            }

            if (GetItem().item_info != Inventory.clicked_item.item_info)
            {
                //Switch Items
                //CreateClickedItem();
                InventoryItem ii = Inventory.clicked_item;
                ClearClickedItem();
                CreateClickedItem();
                NewSlot(inventory_base.inventory_items, (int)ii.item_info.unique_id, ii.amount);
                return;
            }

            if (
                GetAmountOfSlot(inventory_base.inventory_items)
                == Inventory.clicked_item.item_info.max_slot_amount
            )
                return;

            if (ItemCanBeHalfed(Inventory.clicked_item))
                if (btn.ButtonMask == MouseButtonMask.Right)
                {
                    //Check if 48 Slot overgoes
                    if ( //Check if <= slot max
                        Inventory.clicked_item.amount + GetAmountOfSlot(inventory_base.inventory_items)
                        <= Inventory.clicked_item.item_info.max_slot_amount
                    )
                    {
                        UpdateSlot(
                            inventory_base.inventory_items,
                            GetAmountOfSlot(inventory_base.inventory_items)
                                + HalfAmountNextInt(Inventory.clicked_item.amount)
                        );
                        Inventory.clicked_item.amount = HalfAmount(Inventory.clicked_item.amount);
                    }
                    else
                    {
                        // 48 - 30 = 18
                        int diff =
                            Inventory.clicked_item.item_info.max_slot_amount
                            - GetAmountOfSlot(inventory_base.inventory_items);

                        UpdateSlot(
                            inventory_base.inventory_items,
                            GetAmountOfSlot(inventory_base.inventory_items) + diff
                        );
                        Inventory.clicked_item.amount -= diff;
                    }
                    return;
                }

            if ( //Check if <= slot max
                Inventory.clicked_item.amount + GetAmountOfSlot(inventory_base.inventory_items)
                <= Inventory.clicked_item.item_info.max_slot_amount
            )
            {
                UpdateSlot(
                    inventory_base.inventory_items,
                    GetAmountOfSlot(inventory_base.inventory_items) + Inventory.clicked_item.amount
                );
                ClearClickedItem();
            }
            else
            {
                int diff =
                    Inventory.clicked_item.item_info.max_slot_amount
                    - GetAmountOfSlot(inventory_base.inventory_items);

                UpdateSlot(
                    inventory_base.inventory_items,
                    GetAmountOfSlot(inventory_base.inventory_items) + diff
                );
                Inventory.clicked_item.amount -= diff;
            }
        }
    }

    private void UpdateSlot(ItemSave[] i_save, int amount)
    {
        i_save[GetIndex()].amount = amount;
        inventory_base.UpdateSlotUI(GetIndex(), Inventory.clicked_item);
    }

    private int GetAmountOfSlot(ItemSave[] i_save)
    {
        return i_save[GetIndex()].amount;
    }

    private void NewSlot(ItemSave[] i_save, int id, int amount)
    {
        i_save[GetIndex()] = new ItemSave(id, amount);
        inventory_base.UpdateSlotUI(GetIndex(), Inventory.clicked_item);
    }

    private void ClearSlot(ItemSave[] i_save)
    {
        i_save[GetIndex()] = null;
        inventory_base.UpdateSlotUI(GetIndex());
    }

    private void ClearClickedItem()
    {
        Inventory.clicked_item.Free();
        Inventory.clicked_item = null;
    }

    private int HalfAmount(int amount)
    {
        return (int)(amount / 2.0);
    }

    private int HalfAmountNextInt(int amount)
    {
        return (int)(amount / 2.0 + 0.5f);
    }

    public void OnChestSlotButton(InputEventMouseButton @btn)
    {
        if (Inventory.clicked_item == null)
        {
            if (GetItem() != null)
            {
                if (ItemCanBeHalfed(GetItem()))
                    if (@btn.ButtonMask == MouseButtonMask.Right)
                    {
                        CreateClickedItem(halfNext: true);
                        UpdateSlot(
                            chest.chest_items,
                            HalfAmount(chest.chest_items[GetIndex()].amount)
                        );
                        return;
                    }

                CreateClickedItem();
                ClearSlot(chest.chest_items);
            }
        }
        else
        {
            if (GetItem() == null)
            {
                if (ItemCanBeHalfed(Inventory.clicked_item))
                    if (btn.ButtonMask == MouseButtonMask.Right)
                    {
                        NewSlot(
                            chest.chest_items,
                            (int)Inventory.clicked_item.item_info.unique_id,
                            HalfAmountNextInt(Inventory.clicked_item.amount)
                        );
                        Inventory.clicked_item.amount = (int)(Inventory.clicked_item.amount / 2.0);
                        return;
                    }

                NewSlot(
                    chest.chest_items,
                    (int)Inventory.clicked_item.item_info.unique_id,
                    Inventory.clicked_item.amount
                );
                ClearClickedItem();
                return;
            }

            if (GetItem().item_info != Inventory.clicked_item.item_info)
            {
                //Switch Items
                //CreateClickedItem();
                InventoryItem ii = Inventory.clicked_item;
                ClearClickedItem();
                CreateClickedItem();
                NewSlot(chest.chest_items, (int)ii.item_info.unique_id, ii.amount);
                return;
            }

            if (ItemCanBeHalfed(Inventory.clicked_item))
                if (btn.ButtonMask == MouseButtonMask.Right)
                {
                    //Check if 48 Slot overgoes
                    if ( //Check if <= slot max
                        Inventory.clicked_item.amount + GetAmountOfSlot(chest.chest_items)
                        <= Inventory.clicked_item.item_info.max_slot_amount
                    )
                    {
                        UpdateSlot(
                            chest.chest_items,
                            GetAmountOfSlot(chest.chest_items)
                                + HalfAmountNextInt(Inventory.clicked_item.amount)
                        );
                        Inventory.clicked_item.amount = HalfAmount(Inventory.clicked_item.amount);
                    }
                    else
                    {
                        // 48 - 30 = 18
                        int diff =
                            Inventory.clicked_item.item_info.max_slot_amount
                            - GetAmountOfSlot(chest.chest_items);

                        UpdateSlot(chest.chest_items, GetAmountOfSlot(chest.chest_items) + diff);
                        Inventory.clicked_item.amount -= diff;
                    }
                    return;
                }
            if ( //Check if <= slot max
                Inventory.clicked_item.amount + GetAmountOfSlot(chest.chest_items)
                <= Inventory.clicked_item.item_info.max_slot_amount
            )
            {
                UpdateSlot(
                    chest.chest_items,
                    GetAmountOfSlot(chest.chest_items) + Inventory.clicked_item.amount
                );
                ClearClickedItem();
            }
            else
            {
                int diff =
                    Inventory.clicked_item.item_info.max_slot_amount
                    - GetAmountOfSlot(chest.chest_items);

                UpdateSlot(chest.chest_items, GetAmountOfSlot(chest.chest_items) + diff);
                Inventory.clicked_item.amount -= diff;
            }
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
                    1
                );
                Inventory.clicked_item.amount -= 1;
                if (Inventory.clicked_item.amount == 0)
                    ClearClickedItem();
                ResearchTab.INSTANCE.UpdateLevelTabs();
                return;
            }
        }
    }

    public void CreateClickedItem(bool halfNext = false)
    {
        InventoryItem ii = new InventoryItem();
        ii.init(GetItem().item_info);

        if (!halfNext)
            ii.amount = GetItem().amount;
        else
            ii.amount = HalfAmountNextInt(GetItem().amount);

        ii.MouseFilter = MouseFilterEnum.Ignore;
        ii.ZIndex = 10;
        ii.SelfModulate = GetItem().SelfModulate;
        Inventory.clicked_item = ii;
        Inventory.INSTANCE.AddChild(ii);
    }

    public bool ItemCanBeHalfed(InventoryItem ii)
    {
        if (((int)(ii.amount / 2.0)) > 0)
            return true;
        return false;
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
                            ClearClickedItem();
                            break;

                        case (int)FurnaceTab.SlotType.FUEL:
                            FurnaceTab.INSTANCE.process_building.fuel_item_info = Inventory
                                .clicked_item
                                .item_info;
                            FurnaceTab.INSTANCE.process_building.fuel_count = Inventory
                                .clicked_item
                                .amount;
                            FurnaceTab.INSTANCE.UpdateFurnaceUI();
                            ClearClickedItem();
                            break;
                    }
                }
            }
        }
    }
}
