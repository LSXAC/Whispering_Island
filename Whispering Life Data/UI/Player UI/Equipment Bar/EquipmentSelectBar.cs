using System;
using Godot;
using Godot.Collections;

public partial class EquipmentSelectBar : Container
{
    private const string SELECT_BOX_NODE = "SelectedBox";
    private const string ACTUAL_USE_BOX_NODE = "ActualUseBox";
    private bool is_actual_use_active = false;

    [Export]
    public Color normal_color,
        selected_color;

    [Export]
    public Array<Slot> select_slots = new Array<Slot>();

    public SlotItemUI current_selected_slot_item_ui = null;

    public static int current_selected_slot = 0;
    private readonly FarmlandModificationController farmland_controller =
        new FarmlandModificationController();
    public bool tool_mode_active = false;

    [Export]
    public Texture2D tool_marker_texture;

    [Export]
    private PackedScene building_collider_scene = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://b5u81hi3w1yvx")
    );

    public override void _Ready()
    {
        farmland_controller.Initialize(this, tool_marker_texture, building_collider_scene);
        SelectSelectSlot(0);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("NUM1"))
            OnSelectSlotInput(0);
        if (Input.IsActionJustPressed("NUM2"))
            OnSelectSlotInput(1);
        if (Input.IsActionJustPressed("NUM3"))
            OnSelectSlotInput(2);
        if (Input.IsActionJustPressed("NUM4"))
            OnSelectSlotInput(3);
        if (Input.IsActionJustPressed("NUM5"))
            OnSelectSlotInput(4);
        if (Input.IsActionJustPressed("NUM6"))
            OnSelectSlotInput(5);
        if (Input.IsActionJustPressed("NUM7"))
            OnSelectSlotInput(6);
        if (Input.IsActionJustPressed("NUM8"))
            OnSelectSlotInput(7);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (tool_mode_active)
            farmland_controller.ProcessPhysics(current_selected_slot_item_ui);
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
        tool_mode_active = false;
        farmland_controller.HideToolMarker();
    }

    public void SetItemInSelectSlot(int index)
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
        farmland_controller.HideToolMarker();
        select_slots[current_selected_slot].GetParent().GetParent().GetParent<ColorRect>().Color =
            normal_color;
        select_slots[index].GetParent().GetParent().GetParent<ColorRect>().Color = selected_color;
        current_selected_slot_item_ui = select_slots[index].GetSlotItemUI();
        current_selected_slot = index;

        ToolAttribute tool_attr =
            current_selected_slot_item_ui?.item?.info.GetAttributeOrNull<ToolAttribute>();
        farmland_controller.SetCurrentToolAttribute(tool_attr);
        tool_mode_active = false;
        is_actual_use_active = false;
        UpdateSlotBoxVisibility(index, false);
    }

    private void UpdateSlotBoxVisibility(int selected_index, bool show_actual_use_box)
    {
        for (int i = 0; i < select_slots.Count; i++)
        {
            Control select_box = select_slots[i]
                .GetParent()
                .GetNodeOrNull<Control>(SELECT_BOX_NODE);
            Control actual_use_box = select_slots[i]
                .GetParent()
                .GetNodeOrNull<Control>(ACTUAL_USE_BOX_NODE);

            if (select_box != null)
                select_box.Visible = i == selected_index;

            if (actual_use_box != null)
                actual_use_box.Visible = i == selected_index && show_actual_use_box;
        }
    }

    public void ActivateToolMode()
    {
        tool_mode_active = true;
    }

    public void DeactivateToolMode()
    {
        tool_mode_active = false;
        farmland_controller.HideToolMarker();
    }

    public bool IsToolModeActive()
    {
        return tool_mode_active;
    }

    public void HideToolMarker()
    {
        farmland_controller.HideToolMarker();
    }

    public void ResetCurrentSlotToSelectState()
    {
        is_actual_use_active = false;
        DeactivateToolMode();
        UpdateSlotBoxVisibility(current_selected_slot, false);
    }

    public void OnSelectSlotInput(int index)
    {
        HandleSlotKeyInput(index);
    }

    private void HandleSlotKeyInput(int index)
    {
        if (index < 0 || index >= select_slots.Count)
            return;

        if (current_selected_slot != index)
        {
            DeactivateCurrentSlotFunction();
            SelectSelectSlot(index);
            return;
        }

        if (!is_actual_use_active)
        {
            if (!CanCurrentSlotEnterActualUse())
            {
                UpdateSlotBoxVisibility(index, false);
                return;
            }

            if (TriggerSelectedSlotItemAction())
            {
                if (CurrentSlotHasUseAttribute())
                {
                    is_actual_use_active = false;
                    UpdateSlotBoxVisibility(index, false);
                }
                else
                {
                    is_actual_use_active = true;
                    UpdateSlotBoxVisibility(index, true);
                }
            }
            return;
        }

        DeactivateCurrentSlotFunction();
        is_actual_use_active = false;
        UpdateSlotBoxVisibility(index, false);
    }

    private bool CanCurrentSlotEnterActualUse()
    {
        SlotItemUI slot_item_ui = GetSelectedSlotItemUI();
        if (slot_item_ui?.item?.info == null)
            return false;

        UseAttribute use_attr = slot_item_ui.item.info.GetAttributeOrNull<UseAttribute>();
        if (use_attr != null)
            return true;

        ToolAttribute tool_attr = slot_item_ui.item.info.GetAttributeOrNull<ToolAttribute>();
        if (tool_attr != null && tool_attr.has_menu)
            return true;

        BuildingAttribute building_attr =
            slot_item_ui.item.info.GetAttributeOrNull<BuildingAttribute>();
        return building_attr != null;
    }

    private bool CurrentSlotHasUseAttribute()
    {
        SlotItemUI slot_item_ui = GetSelectedSlotItemUI();
        if (slot_item_ui?.item?.info == null)
            return false;

        return slot_item_ui.item.info.GetAttributeOrNull<UseAttribute>() != null;
    }

    private void DeactivateCurrentSlotFunction()
    {
        ItemUseManager.instance?.CancelActiveBuildingPlacement();
        DeactivateToolMode();
    }

    private bool TriggerSelectedSlotItemAction()
    {
        if (CutsceneManager.In_Cutscene || GameMenu.IsWindowActiv())
            return false;

        ItemUseManager.instance?.TriggerEquipmentSlotAction(current_selected_slot);
        return true;
    }
}
