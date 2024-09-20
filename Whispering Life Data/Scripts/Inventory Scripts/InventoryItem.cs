using System;
using System.Diagnostics;
using Godot;

public partial class InventoryItem : TextureRect
{
    [Export]
    public ItemInfo item_info;

    [Export]
    public int amount = 0;
    public Label amount_label;

    public void init(ItemInfo item_info)
    {
        this.Size = new Vector2(32, 32);
        this.item_info = item_info;
        Label l = new Label { ZIndex = 1 };
        amount_label = l;
        TooltipText = TranslationServer.Translate(item_info.item_description.ToString());
        AddChild(l);
    }

    public void UpdateAmountLabel()
    {
        amount_label.Text = amount + "x";
    }

    public override void _Ready()
    {
        Texture = item_info.texture;
        ExpandMode = ExpandModeEnum.IgnoreSize;
        StretchMode = StretchModeEnum.KeepAspectCentered;
    }

    /*public override Variant _GetDragData(Vector2 atPosition)
    {
        SetDragPreview(MakeDragPreview(atPosition));
        return this;
    }

    public TextureRect MakeDragPreview(Vector2 atPosition)
    {
        var preview = new TextureRect
        {
            Texture = this.Texture,
            SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
            SizeFlagsVertical = SizeFlags.ShrinkCenter,
            Position = -atPosition,
        };

        return preview;
    }*/
}
