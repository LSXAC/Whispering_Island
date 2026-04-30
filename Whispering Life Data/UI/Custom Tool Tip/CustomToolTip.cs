using System;
using System.Collections.Generic;
using Godot;

public partial class CustomToolTip : PanelContainer
{
    [Export]
    public Label title_label,
        content_label;

    [Export]
    public VBoxContainer attributes_container;

    [Export]
    public HBoxContainer poisoned_container;

    [Export]
    public string button_content = "";

    [Export]
    public bool position_left = false;

    [Export]
    public Control bounds_container = null;

    const float SCREEN_BORDER_OFFSET = 8f;
    Tween opacityTween = null;
    private bool is_being_destroyed = false;

    public override void _Ready()
    {
        Hide();

        if (title_label == null)
            title_label = GetNode<Label>("VBoxContainer/HBoxContainer/Title");
        if (content_label == null)
            content_label = GetNode<Label>("VBoxContainer/Content");
        if (attributes_container == null)
            attributes_container = GetNode<VBoxContainer>("VBoxContainer/AttributesContainer");

        if (GetParent() is Control)
        {
            ((Control)GetParent()).MouseEntered += () => Toggle(true);
            ((Control)GetParent()).MouseExited += () => Toggle(false);
        }

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

    public override void _Input(InputEvent @event)
    {
        if (!IsNodeReady() || !Visible)
            return;

        if (@event is InputEventMouseMotion)
        {
            Vector2 tooltipSize = GetRect().Size;
            Vector2 newPos;
            Rect2 availableRect;

            if (bounds_container != null && bounds_container.IsNodeReady())
                availableRect = bounds_container.GetGlobalRect();
            else
                availableRect = GetViewportRect();

            if (position_left)
                newPos = GetGlobalMousePosition() + new Vector2(-tooltipSize.X - 10, 10);
            else
                newPos = GetGlobalMousePosition() + new Vector2(10, 10);

            float minX = availableRect.Position.X + SCREEN_BORDER_OFFSET;
            float maxX =
                availableRect.Position.X
                + availableRect.Size.X
                - tooltipSize.X
                - SCREEN_BORDER_OFFSET;
            float minY = availableRect.Position.Y + SCREEN_BORDER_OFFSET;
            float maxY =
                availableRect.Position.Y
                + availableRect.Size.Y
                - tooltipSize.Y
                - SCREEN_BORDER_OFFSET;

            newPos.X = Mathf.Clamp(newPos.X, minX, maxX);
            newPos.Y = Mathf.Clamp(newPos.Y, minY, maxY);

            GlobalPosition = newPos;
        }
    }

    public async void Toggle(bool on)
    {
        if (GameManager.IsGameInterupted() || !IsNodeReady())
            return;

        try
        {
            if (on)
            {
                Show();
                Modulate = new Color(1, 1, 1, 0);
                TweenOpacity(new Color(1, 1, 1, 1));
            }
            else
            {
                if (!IsNodeReady())
                    return;

                Modulate = new Color(1, 1, 1, 1);
                Tween tween = TweenOpacity(new Color(1, 1, 1, 0));
                if (tween != null && IsNodeReady())
                    await ToSignal(tween, Tween.SignalName.Finished);

                if (IsNodeReady())
                    Hide();
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"Error in Toggle: {ex.Message}");
        }
    }

    public void BlockTweens()
    {
        is_being_destroyed = true;
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

            if (IsNodeReady())
                opacityTween.TweenProperty(this, "modulate", to, 0.2f);

            return opacityTween;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error in TweenOpacity: {ex.Message}");
            return null;
        }
    }

    public void SetTitle(string title)
    {
        if (title_label != null)
            title_label.Text = title;
    }

    public void SetContent(string content)
    {
        if (content_label != null)
            content_label.Text = content;
    }

    public void AddContent(string content)
    {
        if (content_label != null)
            content_label.Text += content;
    }

    public void ClearTitle()
    {
        if (title_label != null)
            title_label.Text = "";
    }

    public void ClearContent()
    {
        if (content_label != null)
            content_label.Text = "";
    }

    public void UpdateItemAttributes()
    {
        if (attributes_container == null)
            return;

        foreach (Node child in attributes_container.GetChildren())
        {
            if (child is Control control)
                control.Visible = false;
        }

        if (GetParent() is not SlotItemUI slot_item_ui || slot_item_ui.item?.info == null)
            return;

        foreach (ItemAttributeBase attribute in slot_item_ui.item.info.attributes)
        {
            if (attribute == null)
                continue;

            string attribute_type_name = attribute.GetType().Name;
            Node attribute_node = attributes_container.FindChild(attribute_type_name);

            if (attribute_node is Control control)
            {
                control.Visible = true;
                Label label = control.GetNode<Label>("Label");
                label.Text = attribute.GetNameOfAttribute();
            }
            else
                GD.PrintErr($"Attribute node '{attribute_type_name}' not found in tooltip!");
        }
    }

    public void Update(Item item, int current_durability = -1, int max_durability = 0)
    {
        if (item?.info == null || title_label == null || content_label == null)
            return;

        poisoned_container.Visible = false;
        SetTitle(TranslationServer.Translate(item.info.name.ToString()));

        string content = TranslationServer.Translate(item.info.description.ToString()) + "\n";

        if (current_durability != -1)
            content += "Durability: " + current_durability + "/" + max_durability + "\n";

        if (item.state != Item.STATE.NORMAL)
            poisoned_container.Visible = true;

        SetContent(content);

        UpdateItemAttributesForItem(item);
    }

    private void UpdateItemAttributesForItem(Item item)
    {
        if (item?.info == null || attributes_container == null)
            return;

        foreach (Node child in attributes_container.GetChildren())
        {
            if (child is Control control)
                control.Visible = false;
        }

        foreach (ItemAttributeBase attribute in item.info.attributes)
        {
            if (attribute == null)
                continue;

            string attribute_type_name = attribute.GetType().Name;
            Node attribute_node = attributes_container.FindChild(attribute_type_name);

            if (attribute_node is Control control)
            {
                control.Visible = true;

                Label label = control.GetNode<Label>("Label");
                label.Text = attribute.GetNameOfAttribute();
            }
            else
                GD.PrintErr($"Attribute node '{attribute_type_name}' not found in tooltip!");
        }
    }
}
