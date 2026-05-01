using System;
using System.Diagnostics;
using Godot;

public partial class ItemUseManager : Node2D
{
    public static ItemUseManager instance;
    private int pending_build_slot_index = -1;
    private bool building_placed_signal_connected = false;

    public override void _Ready()
    {
        instance = this;
        TryConnectBuildingPlacedSignal();
    }

    public void TriggerEquipmentSlotAction(int slot_index)
    {
        if (slot_index < 0)
            return;

        Slot equipment_slot = GetEquipmentSlot(slot_index);
        SlotItemUI slot_item_ui = equipment_slot?.GetSlotItemUI();
        if (slot_item_ui?.item?.info == null)
            return;

        UseAttribute use_attr = slot_item_ui.item.info.GetAttributeOrNull<UseAttribute>();
        if (use_attr != null)
        {
            CancelActiveBuildingPlacement();
            UseItem(slot_item_ui.item);
            ConsumeEquippedToolItem(slot_index, 1);
            return;
        }

        ToolAttribute tool_attr = slot_item_ui.item.info.GetAttributeOrNull<ToolAttribute>();
        if (tool_attr != null && tool_attr.has_menu)
        {
            EquipmentSelectBar select_bar = PlayerUI.instance?.equipmentSelectBar;
            if (select_bar != null)
            {
                if (!select_bar.IsToolModeActive())
                    select_bar.ActivateToolMode();
                else
                    select_bar.DeactivateToolMode();
            }

            return;
        }

        BuildingAttribute building_attr =
            slot_item_ui.item.info.GetAttributeOrNull<BuildingAttribute>();
        if (building_attr == null || building_attr.building_menu_list_object == null)
            return;

        // Toggle behavior: if building already active, cancel it; otherwise start new building
        BuildingPlacer building_placer = BuildMenu.instance?.building_placer;
        if (
            building_placer != null
            && (
                BuildingPlacer.current_building != null
                || GameManager.building_mode != GameManager.BuildingMode.None
            )
        )
        {
            CancelActiveBuildingPlacement();
        }
        else
        {
            TryConnectBuildingPlacedSignal();
            if (BuildItem(slot_item_ui.item))
                pending_build_slot_index = slot_index;
            else
                pending_build_slot_index = -1;
        }
    }

    public void CancelActiveBuildingPlacement()
    {
        BuildingPlacer building_placer = BuildMenu.instance?.building_placer;
        if (building_placer == null)
            return;

        if (BuildingPlacer.current_building != null)
            building_placer.CloseMenuWithBuildingSelected();
        else if (GameManager.building_mode != GameManager.BuildingMode.None)
            building_placer.CloseMenuWithNoBuilding();

        pending_build_slot_index = -1;
    }

    public void UseItem(Item item, Node target = null)
    {
        if (item?.info == null)
            return;

        UseAttribute use_attr = item.info.GetAttributeOrNull<UseAttribute>();
        if (use_attr != null && use_attr.HasEffects())
            ApplyUseEffects(use_attr, target);
    }

    public bool BuildItem(Item item)
    {
        if (item?.info == null)
            return false;

        BuildingAttribute building_attr = item.info.GetAttributeOrNull<BuildingAttribute>();
        if (building_attr == null || building_attr.building_menu_list_object == null)
            return false;

        BuildingPlacer building_placer = BuildMenu.instance?.building_placer;
        if (building_placer == null)
            return false;

        CancelActiveBuildingPlacement();

        GameMenu.CloseLastWindow();
        building_placer.InitBuildingFromBuildingMenu(
            building_attr.building_menu_list_object,
            item.amount
        );

        return true;
    }

    private Slot GetEquipmentSlot(int index)
    {
        if (EquipmentPanel.instance == null)
            return null;

        if (index < 0 || index >= EquipmentPanel.instance.slots_tool.Count)
            return null;

        return EquipmentPanel.instance.slots_tool[index];
    }

    private void ConsumeEquippedToolItem(int index, int amount)
    {
        if (EquipmentPanel.instance == null)
            return;

        if (index < 0 || index >= EquipmentPanel.instance.equipped_tools.Length)
            return;

        ItemSave equipped_tool = EquipmentPanel.instance.equipped_tools[index];
        if (equipped_tool == null)
            return;

        equipped_tool.amount -= amount;

        if (equipped_tool.amount > 0)
        {
            UpdateSlotUiAmount(index, equipped_tool.amount);
            return;
        }

        EquipmentPanel.instance.equipped_tools[index] = null;
        EquipmentPanel.instance.ClearToolSlotItem(index);
        PlayerUI.instance?.equipmentSelectBar?.ClearSelectSlot(index);

        if (EquipmentSelectBar.current_selected_slot == index)
        {
            EquipmentSelectBar select_bar = PlayerUI.instance?.equipmentSelectBar;
            if (select_bar != null)
            {
                select_bar.current_selected_slot_item_ui = null;
                select_bar.HideToolMarker();
            }
        }

        EquipmentPanel.instance.CalculateStatsFromEquipment();
    }

    private void UpdateSlotUiAmount(int index, int new_amount)
    {
        Slot equipment_slot =
            EquipmentPanel.instance != null && index < EquipmentPanel.instance.slots_tool.Count
                ? EquipmentPanel.instance.slots_tool[index]
                : null;

        EquipmentSelectBar select_bar = PlayerUI.instance?.equipmentSelectBar;
        Slot select_slot =
            select_bar != null && index < select_bar.select_slots.Count
                ? select_bar.select_slots[index]
                : null;

        UpdateSingleSlotAmount(equipment_slot, new_amount);
        UpdateSingleSlotAmount(select_slot, new_amount);
    }

    private void UpdateSingleSlotAmount(Slot slot, int new_amount)
    {
        SlotItemUI slot_item_ui = slot?.GetSlotItemUI();
        if (slot_item_ui == null)
            return;

        slot_item_ui.item.amount = new_amount;
        slot_item_ui.UpdateAmountLabel();
        slot_item_ui.UpdateToolTip();
    }

    private void TryConnectBuildingPlacedSignal()
    {
        if (building_placed_signal_connected)
            return;

        if (BuildMenu.instance?.building_placer == null)
            return;

        BuildMenu.instance.building_placer.BuildingPlaced += OnBuildingPlacedFromEquipmentSlot;
        building_placed_signal_connected = true;
    }

    private void OnBuildingPlacedFromEquipmentSlot()
    {
        if (pending_build_slot_index < 0)
            return;

        ConsumeEquippedToolItem(pending_build_slot_index, 1);

        if (
            EquipmentPanel.instance == null
            || pending_build_slot_index >= EquipmentPanel.instance.equipped_tools.Length
            || EquipmentPanel.instance.equipped_tools[pending_build_slot_index] == null
        )
            pending_build_slot_index = -1;
    }

    private void ApplyUseEffects(UseAttribute use_attr, Node target)
    {
        if (target == null)
            target = PlayerUI.instance;

        Debug.Print("Item used!");
    }
}
