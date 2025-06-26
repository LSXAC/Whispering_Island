using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Slot : Button
{
    [Export]
    public string label_translation_string;

    [Export]
    public ItemAttributeBase attribute;

    [Export]
    public bool check_attributes = false;

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

            if (attribute is WearableAttribute)
            {
                OnEquipSlotButton(GetIndex());
                // Slot Updater not needed now
            }

            if (attribute is ResearchableAttribute)
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
        SlotItemUI slot_item_ui = new SlotItemUI();
        slot_item_ui.init(item, durability);
        slot_item_ui.UpdateAmountLabel();
        if (durability != -1)
            slot_item_ui.SetDurability(durability);
        AddChild(slot_item_ui);
        if (item.amount == 0)
            ClearSlotItem();
    }

    public void UpdateItem(Item item, int durability)
    {
        if (GetSlotItemUI() != null)
        {
            if (item.info == GetSlotItemUI().item.info)
            {
                if (durability != -1)
                    GetSlotItemUI().SetDurability(durability);

                GetSlotItemUI().item.amount = item.amount;
                GetSlotItemUI().UpdateAmountLabel();
            }
            else
            {
                GetSlotItemUI().QueueFree();
                SetItem(item, durability);
            }
        }
        else
            SetItem(item, durability);
    }

    public SlotItemUI GetSlotItemUI()
    {
        if (GetChildCount() > 0)
            return (SlotItemUI)GetChild(0);
        return null;
    }

    public void ClearSlotItem()
    {
        if (GetSlotItemUI() == null)
            return;
        GetSlotItemUI().QueueFree();
    }

    public void OnEquipSlotButton(int index)
    {
        SlotItemUI slot_item_ui = GetSlotItemUI();
        SlotItemUI clicked_slot_item_ui = PlayerInventoryUI.clicked_slot_item_ui;

        if (PlayerInventoryUI.clicked_slot_item_ui == null)
        {
            if (GetSlotItemUI() != null)
            {
                CreateClickedItem(false, GetSlotItemUI().current_durability);
                if (GetSlotItemUI().item.info.HasAttribute<ToolAttribute>())
                {
                    EquipmentPanel.instance.equipped_tools[index] = null;
                    EquipmentPanel.instance.slots_tool[index].ClearSlotItem();
                    PlayerUI.instance.equipmentSelectBar.select_slots[index].ClearSlotItem();
                    if (EquipmentSelectBar.current_selected_slot == index)
                        PlayerUI.instance.equipmentSelectBar.current_selected_slot_item_ui = null;
                }
                if (GetSlotItemUI().item.info.HasAttribute<ArmorAttribute>())
                {
                    EquipmentPanel.instance.equipped_armor[index] = null;
                    EquipmentPanel.instance.slots_armor[index].ClearSlotItem();
                }
                EquipmentPanel.instance.CalculateStatsFromEquipment();
            }
        }
        else
        {
            if (GetSlotItemUI() == null)
            {
                if (PlayerInventoryUI.clicked_slot_item_ui.item.info.HasAttribute<ArmorAttribute>())
                {
                    OnTakeItemFromArmorSlot(index);
                    return;
                }

                if (PlayerInventoryUI.clicked_slot_item_ui.item.info.HasAttribute<ToolAttribute>())
                {
                    EquipmentPanel.instance.equipped_tools[index] = new ItemSave(
                        (int)PlayerInventoryUI.clicked_slot_item_ui.item.info.id,
                        PlayerInventoryUI.clicked_slot_item_ui.item.amount,
                        PlayerInventoryUI.clicked_slot_item_ui.current_durability
                    );
                    EquipmentPanel
                        .instance.slots_tool[index]
                        .SetItem(
                            PlayerInventoryUI.clicked_slot_item_ui.item,
                            PlayerInventoryUI.clicked_slot_item_ui.current_durability
                        );

                    PlayerUI
                        .instance.equipmentSelectBar.select_slots[index]
                        .SetItem(
                            PlayerInventoryUI.clicked_slot_item_ui.item,
                            PlayerInventoryUI.clicked_slot_item_ui.current_durability
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
            (int)PlayerInventoryUI.clicked_slot_item_ui.item.info.id,
            PlayerInventoryUI.clicked_slot_item_ui.item.amount,
            PlayerInventoryUI.clicked_slot_item_ui.current_durability
        );
        EquipmentPanel
            .instance.slots_armor[index]
            .SetItem(PlayerInventoryUI.clicked_slot_item_ui.item);
        ClearClickedItem();
        EquipmentPanel.instance.CalculateStatsFromEquipment();
    }

    public void OnSlotButton(InputEventMouseButton @btn)
    {
        if (PlayerInventoryUI.clicked_slot_item_ui == null)
        {
            if (GetSlotItemUI() != null)
            {
                if (ItemCanBeHalfed(GetSlotItemUI()))
                    if (@btn.ButtonMask == MouseButtonMask.Right)
                    {
                        CreateClickedItem(halfNext: true);
                        ///////////////////////
                        UpdateSlot(item_array, HalfAmount(item_array[GetIndex()].amount));
                        ///////////////////////
                        return;
                    }
                ToolAttribute attribute = GetSlotItemUI()
                    .item.info.GetAttributeOrNull<ToolAttribute>();
                if (attribute != null)
                    CreateClickedItem(false, GetSlotItemUI().current_durability);
                else
                    CreateClickedItem();
                ///////////////////////
                ClearSlot(item_array);
                ///////////////////////
            }
        }
        else
        {
            if (GetSlotItemUI() == null)
            {
                if (check_attributes)
                    if (
                        !PlayerInventoryUI.clicked_slot_item_ui.item.info.HasAttributByType(
                            attribute.GetType()
                        )
                    )
                        return;
                if (is_export_slot)
                    return;

                if (PlayerInventoryUI.clicked_slot_item_ui.item.amount > 0)
                    if (btn.ButtonMask == MouseButtonMask.Right)
                    {
                        NewSlot(
                            item_array,
                            (int)PlayerInventoryUI.clicked_slot_item_ui.item.info.id,
                            1,
                            PlayerInventoryUI.clicked_slot_item_ui.current_durability
                        );
                        PlayerInventoryUI.clicked_slot_item_ui.item.amount -= 1;
                        return;
                    }

                NewSlot(
                    item_array,
                    (int)PlayerInventoryUI.clicked_slot_item_ui.item.info.id,
                    PlayerInventoryUI.clicked_slot_item_ui.item.amount,
                    PlayerInventoryUI.clicked_slot_item_ui.current_durability
                );
                ClearClickedItem();
                return;
            }

            if (slot_is_switchable)
            {
                if (check_attributes)
                    if (
                        !PlayerInventoryUI.clicked_slot_item_ui.item.info.HasAttributByType(
                            attribute.GetType()
                        )
                    )
                        return;

                if (GetSlotItemUI().item.info != PlayerInventoryUI.clicked_slot_item_ui.item.info)
                {
                    //Switch Items
                    SlotItemUI slot_item = PlayerInventoryUI.clicked_slot_item_ui;
                    ClearClickedItem();
                    CreateClickedItem();
                    NewSlot(
                        item_array,
                        (int)slot_item.item.info.id,
                        slot_item.item.amount,
                        slot_item.current_durability
                    );
                    return;
                }
            }

            if (
                GetAmountOfSlot(item_array)
                == PlayerInventoryUI.clicked_slot_item_ui.item.info.max_stackable_size
            )
                return;

            if (ItemCanBeHalfed(PlayerInventoryUI.clicked_slot_item_ui))
                if (btn.ButtonMask == MouseButtonMask.Right)
                {
                    if (
                        PlayerInventoryUI.clicked_slot_item_ui.item.amount
                            + GetAmountOfSlot(item_array)
                        <= PlayerInventoryUI.clicked_slot_item_ui.item.info.max_stackable_size
                    )
                    {
                        UpdateSlot(item_array, GetAmountOfSlot(item_array) + 1);
                        PlayerInventoryUI.clicked_slot_item_ui.item.amount -= 1;
                    }
                    else
                    {
                        // 48 - 30 = 18
                        int diff =
                            PlayerInventoryUI.clicked_slot_item_ui.item.info.max_stackable_size
                            - GetAmountOfSlot(item_array);

                        UpdateSlot(item_array, GetAmountOfSlot(item_array) + diff);
                        PlayerInventoryUI.clicked_slot_item_ui.item.amount -= diff;
                    }
                    return;
                }

            if ( //Check if <= slot max
                PlayerInventoryUI.clicked_slot_item_ui.item.amount + GetAmountOfSlot(item_array)
                <= PlayerInventoryUI.clicked_slot_item_ui.item.info.max_stackable_size
            )
            {
                UpdateSlot(
                    item_array,
                    GetAmountOfSlot(item_array) + PlayerInventoryUI.clicked_slot_item_ui.item.amount
                );
                ClearClickedItem();
            }
            else
            {
                int diff =
                    PlayerInventoryUI.clicked_slot_item_ui.item.info.max_stackable_size
                    - GetAmountOfSlot(item_array);

                UpdateSlot(item_array, GetAmountOfSlot(item_array) + diff);
                PlayerInventoryUI.clicked_slot_item_ui.item.amount -= diff;
            }
        }
    }

    private void UpdateSlot(ItemSave[] i_save, int amount)
    {
        i_save[GetIndex()].amount = amount;
        slotUpdater.UpdateSlot(GetIndex(), PlayerInventoryUI.clicked_slot_item_ui);
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
        slotUpdater.UpdateSlot(GetIndex(), PlayerInventoryUI.clicked_slot_item_ui);
    }

    private void ClearSlot(ItemSave[] i_save)
    {
        i_save[GetIndex()] = null;
        slotUpdater.ClearSlot(GetIndex());
    }

    private void ClearClickedItem()
    {
        PlayerInventoryUI.clicked_slot_item_ui.Free();
        PlayerInventoryUI.clicked_slot_item_ui = null;
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
        if (PlayerInventoryUI.clicked_slot_item_ui == null)
        {
            if (GetSlotItemUI() != null)
            {
                CreateClickedItem();
                ResearchTab.research_slot_item = null;
                ResearchTab.instance.UpdateLevelTabs();
            }
        }
        else
        {
            if (
                !PlayerInventoryUI.clicked_slot_item_ui.item.info.HasAttribute<ResearchableAttribute>()
            )
                return;

            if (GetSlotItemUI() == null)
            {
                ResearchTab.research_slot_item = new ItemSave(
                    (int)PlayerInventoryUI.clicked_slot_item_ui.item.info.id,
                    1
                );
                PlayerInventoryUI.clicked_slot_item_ui.item.amount -= 1;
                if (PlayerInventoryUI.clicked_slot_item_ui.item.amount == 0)
                    ClearClickedItem();
                ResearchTab.instance.UpdateLevelTabs();
            }
        }
    }

    public void CreateClickedItem(bool halfNext = false, int durability = -1)
    {
        SlotItemUI slot_item_ui = new SlotItemUI();
        slot_item_ui.init(GetSlotItemUI().item, durability);
        Debug.Print("Durability: " + durability);
        if (!halfNext)
            slot_item_ui.item.amount = GetSlotItemUI().item.amount;
        else
            slot_item_ui.item.amount = HalfAmountNextInt(GetSlotItemUI().item.amount);

        slot_item_ui.MouseFilter = MouseFilterEnum.Ignore;
        slot_item_ui.ZIndex = 10;
        if (durability != -1)
            slot_item_ui.SetDurability(durability);
        slot_item_ui.SelfModulate = GetSlotItemUI().SelfModulate;
        PlayerInventoryUI.clicked_slot_item_ui = slot_item_ui;
        PlayerInventoryUI.instance.AddChild(slot_item_ui);
    }

    public bool ItemCanBeHalfed(SlotItemUI slot_item_ui)
    {
        if (((int)(slot_item_ui.item.amount / 2.0)) > 0)
            return true;
        return false;
    }
}
