using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Slot : Button
{
    [Export]
    public string label_translation_string;

    [Export]
    public ItemInfo.Type slot_type;

    [Export]
    public bool check_slot_type = false;

    [Export]
    public bool is_export_slot = false;

    [Export]
    public bool slot_is_switchable = true;

    ItemSave[] item_array = null;
    SlotUpdater slotUpdater;
    ChestBase chest = null;

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton btn && @event.IsPressed())
        {
            if (btn.ButtonMask != MouseButtonMask.Left && btn.ButtonMask != MouseButtonMask.Right)
                return;

            if (GetParent().GetParent() is Inventory)
            {
                item_array = ((InventoryBase)GetParent().GetParent()).inventory_items;
                slotUpdater = (InventoryBase)GetParent().GetParent();
                OnSlotButton(btn);
            }

            if (GetParent().GetParent().GetParent() is FurnaceTab)
            {
                item_array = ((FurnaceTab)GetParent().GetParent().GetParent())
                    .process_building
                    .item_array;
                slotUpdater = (FurnaceTab)GetParent().GetParent().GetParent();
                if (item_array != null)
                    OnSlotButton(btn);
            }

            if (GetParent().GetParent() is ChestInventory)
            {
                item_array = ((InventoryBase)GetParent().GetParent()).inventory_items;
                slotUpdater = (InventoryBase)GetParent().GetParent();
                chest = ChestInventory.current_chest;
                OnSlotButton(btn);
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
                // Slot Updater not needed now
            }

            if (slot_type == ItemInfo.Type.RESEARCHABLE)
            {
                OnResearchSlotButton();
                //SlotUpdater not needed now
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

    public void SetItem(ItemInfo item_info, int amount, int durability = -1)
    {
        InventoryItem ii = new InventoryItem();
        ii.init(item_info, durability);
        ii.amount = amount;
        ii.UpdateAmountLabel();
        if (durability != -1)
            ii.SetDurability(durability);
        AddChild(ii);
        if (amount == 0)
            ClearItem();
    }

    public void UpdateItem(ItemInfo item_info, int amount, int durability)
    {
        if (GetItem() != null)
        {
            if (item_info == GetItem().item_info)
            {
                if (durability != -1)
                    GetItem().SetDurability(durability);

                GetItem().amount = amount;
                GetItem().UpdateAmountLabel();
            }
            else
            {
                GetItem().QueueFree();
                SetItem(item_info, amount, durability);
            }
        }
        else
            SetItem(item_info, amount, durability);
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
                Debug.Print(GetItem().current_durability.ToString());
                CreateClickedItem(false, GetItem().current_durability);
                if (GetItem().item_info.HasType(ItemInfo.Type.TOOL))
                {
                    EquipmentPanel.INSTANCE.equipped_tools[index] = null;
                    EquipmentPanel.INSTANCE.slots_tool[index].ClearItem();
                    player_ui.INSTANCE.equipmentSelectBar.select_slots[index].ClearItem();
                    if (EquipmentSelectBar.current_selected_slot == index)
                        player_ui.INSTANCE.equipmentSelectBar.current_selected_item = null;
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
                        Inventory.clicked_item.amount,
                        Inventory.clicked_item.current_durability
                    );
                    EquipmentPanel
                        .INSTANCE.slots_tool[index]
                        .SetItem(
                            Inventory.clicked_item.item_info,
                            Inventory.clicked_item.amount,
                            Inventory.clicked_item.current_durability
                        );

                    player_ui
                        .INSTANCE.equipmentSelectBar.select_slots[index]
                        .SetItem(
                            Inventory.clicked_item.item_info,
                            Inventory.clicked_item.amount,
                            Inventory.clicked_item.current_durability
                        );
                    if (EquipmentSelectBar.current_selected_slot == index)
                        player_ui.INSTANCE.equipmentSelectBar.SelectSelectSlot(index);
                    ClearClickedItem();
                    EquipmentPanel.INSTANCE.CalculateStatsFromEquipment();
                }
            }
            else // Can be Expanded, if e.g. Stackable Potions in one Equipment Slot
                return;
        }
    }

    private void OnTakeItemFromArmorSlot(int index)
    {
        EquipmentPanel.INSTANCE.equipped_armor[index] = new ItemSave(
            (int)Inventory.clicked_item.item_info.unique_id,
            Inventory.clicked_item.amount,
            Inventory.clicked_item.current_durability
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
                        ///////////////////////
                        UpdateSlot(item_array, HalfAmount(item_array[GetIndex()].amount));
                        ///////////////////////
                        return;
                    }

                if (GetItem().item_info.has_durability)
                    CreateClickedItem(false, GetItem().current_durability);
                else
                    CreateClickedItem();
                ///////////////////////
                ClearSlot(item_array);
                ///////////////////////
            }
        }
        else
        {
            if (GetItem() == null)
            {
                if (check_slot_type)
                    if (!Inventory.clicked_item.item_info.HasType(slot_type))
                        return;
                if (is_export_slot)
                    return;

                if (Inventory.clicked_item.amount > 0)
                    if (btn.ButtonMask == MouseButtonMask.Right)
                    {
                        NewSlot(
                            item_array,
                            (int)Inventory.clicked_item.item_info.unique_id,
                            1,
                            Inventory.clicked_item.current_durability
                        );
                        Inventory.clicked_item.amount -= 1;
                        return;
                    }

                NewSlot(
                    item_array,
                    (int)Inventory.clicked_item.item_info.unique_id,
                    Inventory.clicked_item.amount,
                    Inventory.clicked_item.current_durability
                );
                ClearClickedItem();
                return;
            }

            if (slot_is_switchable)
            {
                if (check_slot_type)
                    if (!Inventory.clicked_item.item_info.HasType(slot_type))
                        return;

                if (GetItem().item_info != Inventory.clicked_item.item_info)
                {
                    //Switch Items
                    InventoryItem ii = Inventory.clicked_item;
                    ClearClickedItem();
                    CreateClickedItem();
                    NewSlot(
                        item_array,
                        (int)ii.item_info.unique_id,
                        ii.amount,
                        ii.current_durability
                    );
                    return;
                }
            }

            if (GetAmountOfSlot(item_array) == Inventory.clicked_item.item_info.max_slot_amount)
                return;

            if (ItemCanBeHalfed(Inventory.clicked_item))
                if (btn.ButtonMask == MouseButtonMask.Right)
                {
                    if (
                        Inventory.clicked_item.amount + GetAmountOfSlot(item_array)
                        <= Inventory.clicked_item.item_info.max_slot_amount
                    )
                    {
                        UpdateSlot(item_array, GetAmountOfSlot(item_array) + 1);
                        Inventory.clicked_item.amount -= 1;
                    }
                    else
                    {
                        // 48 - 30 = 18
                        int diff =
                            Inventory.clicked_item.item_info.max_slot_amount
                            - GetAmountOfSlot(item_array);

                        UpdateSlot(item_array, GetAmountOfSlot(item_array) + diff);
                        Inventory.clicked_item.amount -= diff;
                    }
                    return;
                }

            if ( //Check if <= slot max
                Inventory.clicked_item.amount + GetAmountOfSlot(item_array)
                <= Inventory.clicked_item.item_info.max_slot_amount
            )
            {
                UpdateSlot(item_array, GetAmountOfSlot(item_array) + Inventory.clicked_item.amount);
                ClearClickedItem();
            }
            else
            {
                int diff =
                    Inventory.clicked_item.item_info.max_slot_amount - GetAmountOfSlot(item_array);

                UpdateSlot(item_array, GetAmountOfSlot(item_array) + diff);
                Inventory.clicked_item.amount -= diff;
            }
        }
    }

    private void UpdateSlot(ItemSave[] i_save, int amount)
    {
        i_save[GetIndex()].amount = amount;
        slotUpdater.UpdateSlot(GetIndex(), Inventory.clicked_item);
    }

    private int GetAmountOfSlot(ItemSave[] i_save)
    {
        return i_save[GetIndex()].amount;
    }

    private void NewSlot(ItemSave[] i_save, int id, int amount, int durability = -1)
    {
        if (durability != -1)
            i_save[GetIndex()] = new ItemSave(id, amount, durability);
        else
            i_save[GetIndex()] = new ItemSave(id, amount);
        slotUpdater.UpdateSlot(GetIndex(), Inventory.clicked_item);
    }

    private void ClearSlot(ItemSave[] i_save)
    {
        i_save[GetIndex()] = null;
        slotUpdater.ClearSlot(GetIndex());
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
            }
        }
    }

    public void CreateClickedItem(bool halfNext = false, int durability = -1)
    {
        InventoryItem ii = new InventoryItem();
        ii.init(GetItem().item_info, durability);
        Debug.Print("Durability: " + durability);
        if (!halfNext)
            ii.amount = GetItem().amount;
        else
            ii.amount = HalfAmountNextInt(GetItem().amount);

        ii.MouseFilter = MouseFilterEnum.Ignore;
        ii.ZIndex = 10;
        if (durability != -1)
            ii.SetDurability(durability);
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
}
