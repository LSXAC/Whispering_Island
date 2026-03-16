using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class Slot : Button
{
    [Export]
    public string label_translation_string;

    [Export]
    public ItemAttributeBase attribute_to_check;

    [Export]
    public bool check_attributes = false;

    [Export]
    public bool is_export_slot = false;

    [Export]
    public bool slot_is_switchable = true;
    private int index = 0;

    ItemSave[] item_array = null;
    SlotUpdater slotUpdater;
    ChestBase chest = null;
    PackedScene slot_item_ui_scene = GD.Load<PackedScene>(
        ResourceUid.UidToPath("uid://xjy2l41obahq")
    );

    public override void _Ready()
    {
        if (slot_item_ui_scene == null)
            GD.PrintErr("Slot: slot_item_ui_scene is null, please check if the path is correct.");
    }

    public Action OnSlotChanged;

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton btn && @event.IsPressed())
        {
            if (btn.ButtonMask != MouseButtonMask.Left && btn.ButtonMask != MouseButtonMask.Right)
                return;

            index = GetParent().GetIndex();

            if (GetParent().GetParent().GetParent() is PlayerInventoryUI)
            {
                item_array = ((Inventory)GetParent().GetParent().GetParent()).inventory_items;
                slotUpdater = (Inventory)GetParent().GetParent().GetParent();
                OnSlotButton(btn);
                ((Inventory)GetParent().GetParent().GetParent()).OnItemChanged?.Invoke();
            }

            if (GetParent().GetParent().GetParent().GetParent() is FurnaceTab)
            {
                item_array = ((FurnaceTab)GetParent().GetParent().GetParent().GetParent())
                    .process_building
                    .item_array;
                slotUpdater = (FurnaceTab)GetParent().GetParent().GetParent().GetParent();
                if (item_array != null)
                    OnSlotButton(btn);
            }

            if (GetParent().GetParent().GetParent() is ChestInventory)
            {
                item_array = ((Inventory)GetParent().GetParent().GetParent()).inventory_items;
                slotUpdater = (Inventory)GetParent().GetParent().GetParent();
                chest = ChestInventory.current_chest;
                OnSlotButton(btn);
                ((Inventory)GetParent().GetParent().GetParent()).OnItemChanged?.Invoke();
            }

            if (attribute_to_check is WearableAttribute)
                OnEquipSlotButton(index);

            if (attribute_to_check is ResearchableAttribute)
                OnResearchSlotButton();

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

    public void OnEquipSlotButton(int index)
    {
        SlotItemUI slot_item_ui = GetSlotItemUI();
        SlotItemUI clicked_slot_item_ui = PlayerInventoryUI.clicked_slot_item_ui;

        if (PlayerInventoryUI.clicked_slot_item_ui == null)
        {
            if (slot_item_ui != null) //Take Item from Slot
            {
                CreateClickedItem(
                    HalfAmountNextInt(slot_item_ui.item.amount),
                    slot_item_ui.current_durability
                );
                if (slot_item_ui.item.info.HasAttribute<ToolAttribute>())
                {
                    EquipmentPanel.instance.ClearToolSlotItem(index);
                    PlayerUI.instance.equipmentSelectBar.ClearSelectSlot(index);
                }
                if (slot_item_ui.item.info.HasAttribute<ArmorAttribute>())
                    EquipmentPanel.instance.ClearArmorSlotItem(index);

                EquipmentPanel.instance.CalculateStatsFromEquipment();
            }
        }
        else
        {
            //TODO: ClearItem used twice, maybe refactor
            if (slot_item_ui == null)
            {
                WearableAttribute wearable =
                    clicked_slot_item_ui.item.info.GetAttributeOrNull<WearableAttribute>();

                if (wearable == null)
                    return;

                if (wearable.slot_type != ((WearableAttribute)attribute_to_check).slot_type)
                    return;

                if (clicked_slot_item_ui.item.info.HasAttribute<ArmorAttribute>())
                {
                    EquipmentPanel.instance.SetArmorSlotItem(index, clicked_slot_item_ui);
                    ClearClickedItem();
                }
                else if (clicked_slot_item_ui.item.info.HasAttribute<ToolAttribute>())
                {
                    EquipmentPanel.instance.SetToolSlotItem(index, clicked_slot_item_ui);
                    PlayerUI.instance.equipmentSelectBar.SetItemInSelectSlot(
                        index,
                        clicked_slot_item_ui
                    );
                    ClearClickedItem();
                }

                EquipmentPanel.instance.CalculateStatsFromEquipment();
            }
            else // Can be Expanded, if e.g. Stackable Potions in one Equipment Slot
                return;
        }
    }

    public void OnSlotButton(InputEventMouseButton @btn)
    {
        SlotItemUI slot_item_ui = GetSlotItemUI();
        SlotItemUI clicked_slot_item_ui = PlayerInventoryUI.clicked_slot_item_ui;

        if (clicked_slot_item_ui == null)
        {
            if (slot_item_ui != null) //Take Item from Slot
                TakeItemFromSlot(@btn, slot_item_ui);
        }
        else
        {
            if (!CheckSlotRequirements())
                return;

            if (slot_item_ui == null)
            {
                if (btn.ButtonMask == MouseButtonMask.Right)
                {
                    Debug.Print("Rightclick Slot 1 Item");
                    NewSlotItem(
                        (int)clicked_slot_item_ui.item.info.id,
                        1,
                        clicked_slot_item_ui.current_durability
                    );
                    clicked_slot_item_ui.item.amount -= 1;
                    clicked_slot_item_ui.UpdateAmountLabel();
                    if (clicked_slot_item_ui.item.amount <= 0)
                        ClearClickedItem();
                    return;
                }

                NewSlotItem(
                    (int)clicked_slot_item_ui.item.info.id,
                    clicked_slot_item_ui.item.amount,
                    clicked_slot_item_ui.current_durability
                );
                ClearClickedItem();
                return;
            }

            if (slot_is_switchable)
            {
                if (GetSlotItemUI().item.info != clicked_slot_item_ui.item.info)
                {
                    Debug.Print("Switch Slot");
                    ClearClickedItem();
                    CreateClickedItem(slot_item_ui.item.amount, slot_item_ui.current_durability);

                    NewSlotItem(
                        (int)clicked_slot_item_ui.item.info.id,
                        clicked_slot_item_ui.item.amount,
                        clicked_slot_item_ui.current_durability
                    );
                    return;
                }
            }

            if (GetAmountOfSlot(item_array) == clicked_slot_item_ui.item.info.max_stackable_size)
                return;

            if (btn.ButtonMask == MouseButtonMask.Right)
            {
                Debug.Print("Rightclick Slot");
                UpdateSlot(item_array, GetAmountOfSlot(item_array) + 1);
                PlayerInventoryUI.clicked_slot_item_ui.item.amount -= 1;
                PlayerInventoryUI.clicked_slot_item_ui.UpdateAmountLabel();
                if (PlayerInventoryUI.clicked_slot_item_ui.item.amount <= 0)
                    ClearClickedItem();
                return;
            }

            if ( //Check if <= slot max
                clicked_slot_item_ui.item.amount + GetAmountOfSlot(item_array)
                <= clicked_slot_item_ui.item.info.max_stackable_size
            )
            {
                UpdateSlot(
                    item_array,
                    GetAmountOfSlot(item_array) + clicked_slot_item_ui.item.amount
                );
                ClearClickedItem();
            }
            else
            {
                int diff =
                    clicked_slot_item_ui.item.info.max_stackable_size - GetAmountOfSlot(item_array);

                UpdateSlot(item_array, GetAmountOfSlot(item_array) + diff);
                clicked_slot_item_ui.item.amount -= diff;
                clicked_slot_item_ui.UpdateAmountLabel();
            }
        }
    }

    private void TakeItemFromSlot(InputEventMouseButton @btn, SlotItemUI slot_item_ui)
    {
        if (ItemCanBeHalfed(slot_item_ui))
            if (@btn.ButtonMask == MouseButtonMask.Right)
            {
                int first = slot_item_ui.item.amount;
                int half = GetHalfOfAmount(first);
                UpdateSlot(item_array, half);
                CreateClickedItem(first - half);

                Debug.Print("Take Item Rightclick Slot");
                return;
            }

        if (
            slot_item_ui.item.info.HasAttribute<WearableAttribute>()
            || slot_item_ui.item.info.HasAttribute<ToolAttribute>()
        )
            CreateClickedItem(
                HalfAmountNextInt(slot_item_ui.item.amount),
                slot_item_ui.current_durability
            );
        else
            CreateClickedItem(slot_item_ui.item.amount);

        Debug.Print("Take Item Normal");
        ClearSlot(item_array);
    }

    private bool CheckSlotRequirements()
    {
        if (is_export_slot)
            return false;

        if (check_attributes)
            if (
                !PlayerInventoryUI.clicked_slot_item_ui.item.info.HasAttributByType(
                    attribute_to_check.GetType()
                )
            )
                return false;

        return true;
    }

    public void SetItem(Item item, int durability = -1)
    {
        SlotItemUI slot_item_ui = slot_item_ui_scene.Instantiate() as SlotItemUI;
        AddChild(slot_item_ui);
        slot_item_ui.init(item, durability);
        slot_item_ui.UpdateAmountLabel();
        if (durability != -1)
        {
            GetSlotItemUI().RescaleDurabilityBar();
            slot_item_ui.SetDurability(durability);
        }
        if (item.amount == 0)
            ClearSlotItem();
    }

    public void UpdateItem(Item item, int durability)
    {
        if (GetSlotItemUI() == null)
        {
            SetItem(item, durability);
            return;
        }

        if (item.info == GetSlotItemUI().item.info)
        {
            if (durability != -1)
            {
                GetSlotItemUI().RescaleDurabilityBar();
                GetSlotItemUI().SetDurability(durability);
            }

            GetSlotItemUI().item.amount = item.amount;
            GetSlotItemUI().UpdateAmountLabel();
            GetSlotItemUI().UpdateToolTip();
        }
        else
        {
            GetSlotItemUI().QueueFree();
            SetItem(item, durability);
        }
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

    private void UpdateSlot(ItemSave[] i_save, int amount)
    {
        if (amount == -1)
            return;
        i_save[index].amount = amount;
        slotUpdater.UpdateSlot(index, PlayerInventoryUI.clicked_slot_item_ui);
    }

    private int GetAmountOfSlot(ItemSave[] i_save)
    {
        if (item_array[index] == null)
            return -1;
        return i_save[index].amount;
    }

    private void NewSlotItem(int id, int amount, int durability)
    {
        if (durability != -1)
            item_array[index] = new ItemSave(id, amount, durability);
        else
            item_array[index] = new ItemSave(id, amount);
        slotUpdater.UpdateSlot(index, PlayerInventoryUI.clicked_slot_item_ui);
    }

    private void ClearSlot(ItemSave[] i_save)
    {
        i_save[index] = null;
        slotUpdater.ClearSlot(index);
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

    private int GetHalfOfAmount(int amount)
    {
        if (amount % 2 == 0)
            return HalfAmount(amount);
        return HalfAmountNextInt(amount);
    }

    public void OnResearchSlotButton()
    {
        SlotItemUI slot_item_ui = GetSlotItemUI();
        SlotItemUI clicked_slot_item_ui = PlayerInventoryUI.clicked_slot_item_ui;

        if (clicked_slot_item_ui == null)
        {
            if (slot_item_ui != null)
            {
                CreateClickedItem(slot_item_ui.item.amount);
                ResearchTab.research_slot_item = null;
                //ResearchTab.instance.UpdateLevelTabs();
            }
        }
        else
        {
            if (!clicked_slot_item_ui.item.info.HasAttribute<ResearchableAttribute>())
                return;

            if (slot_item_ui == null)
            {
                ResearchTab.research_slot_item = new ItemSave(
                    (int)clicked_slot_item_ui.item.info.id,
                    1
                );
                clicked_slot_item_ui.item.amount -= 1;
                if (clicked_slot_item_ui.item.amount == 0)
                    ClearClickedItem();
                //ResearchTab.instance.UpdateLevelTabs();
            }
        }
    }

    public void CreateClickedItem(int amount, int durability = -1)
    {
        SlotItemUI slot_item_ui = slot_item_ui_scene.Instantiate() as SlotItemUI;
        PlayerInventoryUI.instance.AddChild(slot_item_ui);
        slot_item_ui.init(GetSlotItemUI().item, durability);
        slot_item_ui.item.amount = amount;
        slot_item_ui.MouseFilter = MouseFilterEnum.Ignore;
        slot_item_ui.ZIndex = 10;

        if (durability != -1)
            slot_item_ui.SetDurability(durability);
        slot_item_ui.SelfModulate = GetSlotItemUI().SelfModulate;
        PlayerInventoryUI.clicked_slot_item_ui = slot_item_ui;
        PlayerInventoryUI.clicked_slot_item_ui.UpdateAmountLabel();
    }

    public bool ItemCanBeHalfed(SlotItemUI slot_item_ui)
    {
        if (((int)(slot_item_ui.item.amount / 2.0)) > 0)
            return true;
        return false;
    }
}
