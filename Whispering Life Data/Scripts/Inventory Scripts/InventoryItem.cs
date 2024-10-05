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
        this.Size = new Vector2(40, 40);
        this.Position = new Vector2(4, 4);
        this.item_info = item_info;
        Label l = new Label { ZIndex = 1 };
        l.CustomMinimumSize = new Vector2(42, 46);
        l.VerticalAlignment = VerticalAlignment.Bottom;
        l.HorizontalAlignment = HorizontalAlignment.Right;
        amount_label = l;
        TooltipText = TranslationServer.Translate(item_info.item_name.ToString()) + "\n";
        TooltipText += TranslationServer.Translate(item_info.item_description.ToString()) + "\n";
        foreach (ItemInfo.Type type in item_info.item_types)
        {
            if (type == ItemInfo.Type.BURNABLE)
                TooltipText += "Burntime: " + item_info.burntime + "s" + "\n";
            else
                TooltipText += "Type: " + type + "\n";
        }
        foreach (ItemStats stats in item_info.item_stats)
            if (stats.bonus > 0)
                TooltipText += "\n" + stats.type.ToString() + ": +" + (stats.bonus * 100) + "%";
            else if (stats.bonus < 0)
                TooltipText += "\n" + stats.type.ToString() + ": " + (stats.bonus * 100) + "%";

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
