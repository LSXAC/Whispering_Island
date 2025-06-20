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

            if (GetParent().GetParent() is PlayerInventoryUI)
            {
                item_array = ((Inventory)GetParent().GetParent()).inventory_items;
                slotUpdater = (Inventory)GetParent().GetParent();
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

            if (GetParent().GetParent() is ChestInventoryUI)
            {
                item_array = ((Inventory)GetParent().GetParent()).inventory_items;
                slotUpdater = (Inventory)GetParent().GetParent();
                chest = ChestInventoryUI.current_chest;
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

    public void SetItem(Item item, int durability = -1)
    {
        SlotItem slot_item = new SlotItem();
        slot_item.init(item, durability);
        slot_item.UpdateAmountLabel();
        if (durability != -1)
            slot_item.SetDurability(durability);
        AddChild(slot_item);
        if (item.amount == 0)
            ClearItem();
    }

    public void UpdateItem(Item item, int durability)
    {
        if (GetSlotItem() != null)
        {
            if (item.item_info == GetSlotItem().item.item_info)
            {
                if (durability != -1)
                    GetSlotItem().SetDurability(durability);

                GetSlotItem().item.amount = item.amount;
                GetSlotItem().UpdateAmountLabel();
            }
            else
            {
                GetSlotItem().QueueFree();
                SetItem(item, durability);
            }
        }
        else
            SetItem(item, durability);
    }

    public SlotItem GetSlotItem()
    {
        if (GetChildCount() > 0)
            return (SlotItem)GetChild(0);
        return null;
    }

    public void ClearItem()
    {
        if (GetSlotItem() == null)
            return;
        GetSlotItem().QueueFree();
    }

    public void OnEquipSlotButton(int index)
    {
        if (PlayerInventoryUI.clicked_slot_item == null)
        {
            if (GetSlotItem() != null)
            {
                Debug.Print(GetSlotItem().current_durability.ToString());
                CreateClickedItem(false, GetSlotItem().current_durability);
                if (GetSlotItem().item.item_info.HasType(ItemInfo.Type.TOOL))
                {
                    EquipmentPanel.instance.equipped_tools[index] = null;
                    EquipmentPanel.instance.slots_tool[index].ClearItem();
                    PlayerUI.instance.equipmentSelectBar.select_slots[index].ClearItem();
                    if (EquipmentSelectBar.current_selected_slot == index)
                        PlayerUI.instance.equipmentSelectBar.current_selected_slot_item = null;
                }
                if (
                    GetSlotItem().item.item_info.HasType(ItemInfo.Type.HEAD)
                    || GetSlotItem().item.item_info.HasType(ItemInfo.Type.CHESTPLATE)
                    || GetSlotItem().item.item_info.HasType(ItemInfo.Type.LEGGINGS)
                    || GetSlotItem().item.item_info.HasType(ItemInfo.Type.SHOES)
                )
                {
                    EquipmentPanel.instance.equipped_armor[index] = null;
                    EquipmentPanel.instance.slots_armor[index].ClearItem();
                }
                EquipmentPanel.instance.CalculateStatsFromEquipment();
            }
        }
        else
        {
            if (GetSlotItem() == null)
            {
                if (
                    PlayerInventoryUI.clicked_slot_item.item.item_info.HasType(slot_type)
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
                    PlayerInventoryUI.clicked_slot_item.item.item_info.HasType(slot_type)
                    && slot_type == ItemInfo.Type.TOOL
                )
                {
                    EquipmentPanel.instance.equipped_tools[index] = new ItemSave(
                        (int)PlayerInventoryUI.clicked_slot_item.item.item_info.item_id,
                        PlayerInventoryUI.clicked_slot_item.item.amount,
                        PlayerInventoryUI.clicked_slot_item.current_durability
                    );
                    EquipmentPanel
                        .instance.slots_tool[index]
                        .SetItem(
                            PlayerInventoryUI.clicked_slot_item.item,
                            PlayerInventoryUI.clicked_slot_item.current_durability
                        );

                    PlayerUI
                        .instance.equipmentSelectBar.select_slots[index]
                        .SetItem(
                            PlayerInventoryUI.clicked_slot_item.item,
                            PlayerInventoryUI.clicked_slot_item.current_durability
                        );
                    if (EquipmentSelectBar.current_selected_slot == index)
                        PlayerUI.instance.equipmentSelectBar.SelectSelectSlot(index);
                    ClearClickedItem();
                    EquipmentPanel.instance.CalculateStatsFromEquipment();
                }
            }
            else // Can be Expanded, if e.g. Stackable Potions in one Equipment Slot
                return;
        }
    }

    private void OnTakeItemFromArmorSlot(int index)
    {
        EquipmentPanel.instance.equipped_armor[index] = new ItemSave(
            (int)PlayerInventoryUI.clicked_slot_item.item.item_info.item_id,
            PlayerInventoryUI.clicked_slot_item.item.amount,
            PlayerInventoryUI.clicked_slot_item.current_durability
        );
        EquipmentPanel
            .instance.slots_armor[index]
            .SetItem(PlayerInventoryUI.clicked_slot_item.item);
        ClearClickedItem();
        EquipmentPanel.instance.CalculateStatsFromEquipment();
    }

    public void OnSlotButton(InputEventMouseButton @btn)
    {
        if (PlayerInventoryUI.clicked_slot_item == null)
        {
            if (GetSlotItem() != null)
            {
                if (ItemCanBeHalfed(GetSlotItem()))
                    if (@btn.ButtonMask == MouseButtonMask.Right)
                    {
                        CreateClickedItem(halfNext: true);
                        ///////////////////////
                        UpdateSlot(item_array, HalfAmount(item_array[GetIndex()].amount));
                        ///////////////////////
                        return;
                    }

                if (GetSlotItem().item.item_info.has_durability)
                    CreateClickedItem(false, GetSlotItem().current_durability);
                else
                    CreateClickedItem();
                ///////////////////////
                ClearSlot(item_array);
                ///////////////////////
            }
        }
        else
        {
            if (GetSlotItem() == null)
            {
                if (check_slot_type)
                    if (!PlayerInventoryUI.clicked_slot_item.item.item_info.HasType(slot_type))
                        return;
                if (is_export_slot)
                    return;

                if (PlayerInventoryUI.clicked_slot_item.item.amount > 0)
                    if (btn.ButtonMask == MouseButtonMask.Right)
                    {
                        NewSlot(
                            item_array,
                            (int)PlayerInventoryUI.clicked_slot_item.item.item_info.item_id,
                            1,
                            PlayerInventoryUI.clicked_slot_item.current_durability
                        );
                        PlayerInventoryUI.clicked_slot_item.item.amount -= 1;
                        return;
                    }

                NewSlot(
                    item_array,
                    (int)PlayerInventoryUI.clicked_slot_item.item.item_info.item_id,
                    PlayerInventoryUI.clicked_slot_item.item.amount,
                    PlayerInventoryUI.clicked_slot_item.current_durability
                );
                ClearClickedItem();
                return;
            }

            if (slot_is_switchable)
            {
                if (check_slot_type)
                    if (!PlayerInventoryUI.clicked_slot_item.item.item_info.HasType(slot_type))
                        return;

                if (
                    GetSlotItem().item.item_info
                    != PlayerInventoryUI.clicked_slot_item.item.item_info
                )
                {
                    //Switch Items
                    SlotItem slot_item = PlayerInventoryUI.clicked_slot_item;
                    ClearClickedItem();
                    CreateClickedItem();
                    NewSlot(
                        item_array,
                        (int)slot_item.item.item_info.item_id,
                        slot_item.item.amount,
                        slot_item.current_durability
                    );
                    return;
                }
            }

            if (
                GetAmountOfSlot(item_array)
                == PlayerInventoryUI.clicked_slot_item.item.item_info.max_slot_amount
            )
                return;

            if (ItemCanBeHalfed(PlayerInventoryUI.clicked_slot_item))
                if (btn.ButtonMask == MouseButtonMask.Right)
                {
                    if (
                        PlayerInventoryUI.clicked_slot_item.item.amount
                            + GetAmountOfSlot(item_array)
                        <= PlayerInventoryUI.clicked_slot_item.item.item_info.max_slot_amount
                    )
                    {
                        UpdateSlot(item_array, GetAmountOfSlot(item_array) + 1);
                        PlayerInventoryUI.clicked_slot_item.item.amount -= 1;
                    }
                    else
                    {
                        // 48 - 30 = 18
                        int diff =
                            PlayerInventoryUI.clicked_slot_item.item.item_info.max_slot_amount
                            - GetAmountOfSlot(item_array);

                        UpdateSlot(item_array, GetAmountOfSlot(item_array) + diff);
                        PlayerInventoryUI.clicked_slot_item.item.amount -= diff;
                    }
                    return;
                }

            if ( //Check if <= slot max
                PlayerInventoryUI.clicked_slot_item.item.amount + GetAmountOfSlot(item_array)
                <= PlayerInventoryUI.clicked_slot_item.item.item_info.max_slot_amount
            )
            {
                UpdateSlot(
                    item_array,
                    GetAmountOfSlot(item_array) + PlayerInventoryUI.clicked_slot_item.item.amount
                );
                ClearClickedItem();
            }
            else
            {
                int diff =
                    PlayerInventoryUI.clicked_slot_item.item.item_info.max_slot_amount
                    - GetAmountOfSlot(item_array);

                UpdateSlot(item_array, GetAmountOfSlot(item_array) + diff);
                PlayerInventoryUI.clicked_slot_item.item.amount -= diff;
            }
        }
    }

    private void UpdateSlot(ItemSave[] i_save, int amount)
    {
        i_save[GetIndex()].amount = amount;
        slotUpdater.UpdateSlot(GetIndex(), PlayerInventoryUI.clicked_slot_item);
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
        slotUpdater.UpdateSlot(GetIndex(), PlayerInventoryUI.clicked_slot_item);
    }

    private void ClearSlot(ItemSave[] i_save)
    {
        i_save[GetIndex()] = null;
        slotUpdater.ClearSlot(GetIndex());
    }

    private void ClearClickedItem()
    {
        PlayerInventoryUI.clicked_slot_item.Free();
        PlayerInventoryUI.clicked_slot_item = null;
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
        if (PlayerInventoryUI.clicked_slot_item == null)
        {
            if (GetSlotItem() != null)
            {
                CreateClickedItem();
                ResearchTab.research_slot_item = null;
                ResearchTab.instance.UpdateLevelTabs();
            }
        }
        else
        {
            if (
                !PlayerInventoryUI.clicked_slot_item.item.item_info.HasType(
                    ItemInfo.Type.RESEARCHABLE
                )
            )
                return;

            if (GetSlotItem() == null)
            {
                ResearchTab.research_slot_item = new ItemSave(
                    (int)PlayerInventoryUI.clicked_slot_item.item.item_info.item_id,
                    1
                );
                PlayerInventoryUI.clicked_slot_item.item.amount -= 1;
                if (PlayerInventoryUI.clicked_slot_item.item.amount == 0)
                    ClearClickedItem();
                ResearchTab.instance.UpdateLevelTabs();
            }
        }
    }

    public void CreateClickedItem(bool halfNext = false, int durability = -1)
    {
        SlotItem slot_item = new SlotItem();
        slot_item.init(GetSlotItem().item, durability);
        Debug.Print("Durability: " + durability);
        if (!halfNext)
            slot_item.item.amount = GetSlotItem().item.amount;
        else
            slot_item.item.amount = HalfAmountNextInt(GetSlotItem().item.amount);

        slot_item.MouseFilter = MouseFilterEnum.Ignore;
        slot_item.ZIndex = 10;
        if (durability != -1)
            slot_item.SetDurability(durability);
        slot_item.SelfModulate = GetSlotItem().SelfModulate;
        PlayerInventoryUI.clicked_slot_item = slot_item;
        PlayerInventoryUI.instance.AddChild(slot_item);
    }

    public bool ItemCanBeHalfed(SlotItem slot_item)
    {
        if (((int)(slot_item.item.amount / 2.0)) > 0)
            return true;
        return false;
    }
}
