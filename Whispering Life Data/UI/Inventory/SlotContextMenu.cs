using System;
using Godot;

/// <summary>
/// Context menu for inventory slots with right-click support.
/// Shows options for items with UseAttribute and handles Split Half functionality.
/// Structure: PanelContainer -> VBoxContainer -> UseButton, SplitHalfButton
/// </summary>
public partial class SlotContextMenu : PanelContainer
{
    [Export]
    public Button use_button;

    [Export]
    public Button split_half_button;

    [Export]
    public Button exit_button;

    [Export]
    public VBoxContainer vbox_container;

    [Export]
    public Control bounds_container = null;

    const float SCREEN_BORDER_OFFSET = 8f;
    const float MOUSE_DISTANCE_THRESHOLD = 50;
    Tween opacityTween = null;
    private bool is_being_destroyed = false;
    private bool is_visible_menu = false;
    private bool inTransition = false;
    private Slot parent_slot = null;

    public override void _Ready()
    {
        HideImmediate();
        parent_slot = GetParent() as Slot;

        if (use_button == null)
            use_button = GetNode<Button>("MarginContainer/VBoxContainer/UseButton");
        if (split_half_button == null)
            split_half_button = GetNode<Button>("MarginContainer/VBoxContainer/SplitHalfButton");
        if (exit_button == null)
            exit_button = GetNode<Button>("MarginContainer/VBoxContainer/ExitButton");
        if (vbox_container == null)
            vbox_container = GetNode<VBoxContainer>("MarginContainer/VBoxContainer");

        if (use_button != null)
            use_button.Pressed += OnUsePressed;

        if (split_half_button != null)
            split_half_button.Pressed += OnSplitHalfPressed;

        if (exit_button != null)
            exit_button.Pressed += OnExitPressed;

        if (bounds_container == null)
        {
            Node current = GetParent();
            while (current != null)
            {
                if (current is ScrollContainer scroll_container)
                {
                    bounds_container = scroll_container;
                    break;
                }
                current = current.GetParent();
            }
        }
    }

    public void Show(SlotItemUI slot_item_ui)
    {
        if (slot_item_ui == null || slot_item_ui.item?.info == null)
            return;

        parent_slot = GetParent() as Slot;

        UseAttribute use_attr = slot_item_ui.item.info.GetAttributeOrNull<UseAttribute>();

        if (use_button != null)
            use_button.Visible = use_attr != null;

        if (!IsNodeReady())
            return;

        is_visible_menu = true;
        inTransition = false;
        DisplayMenu();
        PositionMenu();
    }

    private void DisplayMenu()
    {
        if (!IsNodeReady() || GameManager.IsGameInterupted())
            return;

        try
        {
            base.Show();
            Modulate = new Color(1, 1, 1, 0);
            TweenOpacity(new Color(1, 1, 1, 1));
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error in DisplayMenu: {ex.Message}");
        }
    }

    public async void HideMenu()
    {
        if (GameManager.IsGameInterupted() || !IsNodeReady())
        {
            HideImmediate();
            return;
        }

        try
        {
            Modulate = new Color(1, 1, 1, 1);
            Tween tween = TweenOpacity(new Color(1, 1, 1, 0));
            if (tween != null && IsNodeReady())
                await ToSignal(tween, Tween.SignalName.Finished);

            if (IsNodeReady())
                HideImmediate();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error in HideMenu: {ex.Message}");
            HideImmediate();
        }
    }

    private void HideImmediate()
    {
        is_visible_menu = false;
        inTransition = false;
        Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsNodeReady() || !is_visible_menu || Visible == false || inTransition)
            return;

        if (@event is InputEventMouseMotion)
        {
            Vector2 mousePos = GetGlobalMousePosition();
            Rect2 menuRect = GetGlobalRect();

            float distance = float.MaxValue;

            if (
                mousePos.X >= menuRect.Position.X
                && mousePos.X <= menuRect.Position.X + menuRect.Size.X
            )
            {
                if (mousePos.Y < menuRect.Position.Y)
                    distance = menuRect.Position.Y - mousePos.Y;
                else if (mousePos.Y > menuRect.Position.Y + menuRect.Size.Y)
                    distance = mousePos.Y - (menuRect.Position.Y + menuRect.Size.Y);
                else
                    distance = 0;
            }
            else if (
                mousePos.Y >= menuRect.Position.Y
                && mousePos.Y <= menuRect.Position.Y + menuRect.Size.Y
            )
            {
                if (mousePos.X < menuRect.Position.X)
                    distance = menuRect.Position.X - mousePos.X;
                else
                    distance = mousePos.X - (menuRect.Position.X + menuRect.Size.X);
            }
            else
            {
                Vector2 cornerDist = Vector2.Zero;
                if (mousePos.X < menuRect.Position.X)
                    cornerDist.X = menuRect.Position.X - mousePos.X;
                else
                    cornerDist.X = mousePos.X - (menuRect.Position.X + menuRect.Size.X);

                if (mousePos.Y < menuRect.Position.Y)
                    cornerDist.Y = menuRect.Position.Y - mousePos.Y;
                else
                    cornerDist.Y = mousePos.Y - (menuRect.Position.Y + menuRect.Size.Y);

                distance = cornerDist.Length();
            }

            if (distance > MOUSE_DISTANCE_THRESHOLD)
                HideMenu();
        }
    }

    private void PositionMenu()
    {
        if (!IsNodeReady() || parent_slot == null)
            return;

        Vector2 menuSize = GetRect().Size;
        Vector2 slotGlobalPos = parent_slot.GetGlobalRect().Position;
        Vector2 slotSize = parent_slot.GetRect().Size;

        Vector2 newPos = new Vector2(slotGlobalPos.X + slotSize.X, slotGlobalPos.Y + slotSize.Y);

        Rect2 availableRect;
        if (bounds_container != null && bounds_container.IsNodeReady())
            availableRect = bounds_container.GetGlobalRect();
        else
            availableRect = GetViewportRect();

        float minX = availableRect.Position.X + SCREEN_BORDER_OFFSET;
        float maxX =
            availableRect.Position.X + availableRect.Size.X - menuSize.X - SCREEN_BORDER_OFFSET;
        float minY = availableRect.Position.Y + SCREEN_BORDER_OFFSET;
        float maxY =
            availableRect.Position.Y + availableRect.Size.Y - menuSize.Y - SCREEN_BORDER_OFFSET;

        newPos.X = Mathf.Clamp(newPos.X, minX, maxX);
        newPos.Y = Mathf.Clamp(newPos.Y, minY, maxY);

        GlobalPosition = newPos;
    }

    private void OnUsePressed()
    {
        if (parent_slot == null)
        {
            HideMenu();
            return;
        }

        SlotItemUI slot_item_ui = parent_slot.GetSlotItemUI();
        if (slot_item_ui == null || slot_item_ui.item?.info == null)
        {
            HideMenu();
            return;
        }

        UseAttribute use_attr = slot_item_ui.item.info.GetAttributeOrNull<UseAttribute>();
        if (use_attr != null)
        {
            if (ItemUseManager.instance != null)
                ItemUseManager.instance.UseItem(slot_item_ui.item);

            slot_item_ui.item.amount--;
            slot_item_ui.UpdateAmountLabel();

            if (slot_item_ui.item.amount <= 0)
                parent_slot.ClearSlotItem();
        }

        HideMenu();
    }

    private void OnSplitHalfPressed()
    {
        if (parent_slot == null)
        {
            HideMenu();
            return;
        }

        SlotItemUI slot_item_ui = parent_slot.GetSlotItemUI();
        if (slot_item_ui == null)
        {
            HideMenu();
            return;
        }

        parent_slot.SplitItemHalf(slot_item_ui);
        HideMenu();
    }

    private void OnExitPressed()
    {
        HideMenu();
    }

    public void BlockTweens()
    {
        is_being_destroyed = true;
        inTransition = false;
        opacityTween?.Kill();
    }

    public Tween TweenOpacity(Color to)
    {
        if (is_being_destroyed)
            return null;

        if (!IsNodeReady())
            return null;

        var tree = GetTree();
        if (tree == null)
            return null;

        try
        {
            opacityTween?.Kill();
            opacityTween = tree.CreateTween();
            inTransition = true;

            if (IsNodeReady())
                opacityTween.TweenProperty(this, "modulate", to, 0.2f);

            if (opacityTween != null)
                opacityTween.Finished += () =>
                {
                    inTransition = false;
                };

            return opacityTween;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error in TweenOpacity: {ex.Message}");
            inTransition = false;
            return null;
        }
    }
}
