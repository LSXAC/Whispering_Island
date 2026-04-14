using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class EquipmentSelectBar : Container
{
    [Export]
    public Color normal_color,
        selected_color;

    [Export]
    public Array<Slot> select_slots = new Array<Slot>();

    public SlotItemUI current_selected_slot_item_ui = null;

    public static int current_selected_slot = 0;

    public override void _Ready()
    {
        SelectSelectSlot(0);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("NUM1"))
            SelectSelectSlot(0);

        if (Input.IsActionJustPressed("NUM2"))
            SelectSelectSlot(1);

        if (Input.IsActionJustPressed("NUM3"))
            SelectSelectSlot(2);

        if (Input.IsActionJustPressed("NUM4"))
            SelectSelectSlot(3);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsToolWithMenuActive())
            return;

        if (CutsceneManager.In_Cutscene)
            return;

        if (GameMenu.IsWindowActiv())
            return;

        UpdateToolMarker();

        if (Input.IsMouseButtonPressed(MouseButton.Right))
            HandleFarmlandTileModification();

        if (Input.IsMouseButtonPressed(MouseButton.Left))
            HandleFarmlandTileSet();
    }

    public SlotItemUI GetSelectedSlotItemUI()
    {
        return current_selected_slot_item_ui;
    }

    public void ClearSelectSlot(int index)
    {
        select_slots[index].ClearSlotItem();
        if (current_selected_slot == index)
            current_selected_slot_item_ui = null;
    }

    public void SetItemInSelectSlot(int index, SlotItemUI slot_item_ui)
    {
        select_slots[index]
            .SetItem(
                InventoryTab.clicked_slot_item_ui.item,
                InventoryTab.clicked_slot_item_ui.current_durability
            );
        if (current_selected_slot == index)
            SelectSelectSlot(index);
    }

    public MineableObject.MINING_LEVEL GetSelectedTypeLevel()
    {
        if (GetSelectedSlotItemUI() != null)
        {
            ToolAttribute attribute = GetSelectedSlotItemUI()
                .item.info.GetAttributeOrNull<ToolAttribute>();
            if (attribute != null)
                return attribute.mining_level;
        }
        return MineableObject.MINING_LEVEL.HAND;
    }

    public bool HasSameUseType(PlayerStats.TOOLTYPE tool_type)
    {
        if (GetSelectedTypeLevel() == MineableObject.MINING_LEVEL.HAND)
            return true;

        if (GetSelectedSlotItemUI() != null)
        {
            ToolAttribute attribute = GetSelectedSlotItemUI()
                .item.info.GetAttributeOrNull<ToolAttribute>();

            if (attribute != null)
                if (attribute.tool_type == tool_type)
                    return true;
        }
        return false;
    }

    public void SelectSelectSlot(int index)
    {
        tool_marker.Visible = false;
        select_slots[current_selected_slot].GetParent().GetParent().GetParent<ColorRect>().Color =
            normal_color;
        select_slots[index].GetParent().GetParent().GetParent<ColorRect>().Color = selected_color;
        current_selected_slot_item_ui = select_slots[index].GetSlotItemUI();
        current_selected_slot = index;

        if (EquipmentPanel.instance != null)
            EquipmentPanel.instance.CalculateStatsFromEquipment();
    }
}
