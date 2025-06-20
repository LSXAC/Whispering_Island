using System;
using System.Diagnostics;
using Godot;

public partial class SlotItem : TextureRect
{
    public Label amount_label;
    public int current_durability = -1;
    public ProgressBar pb;
    public ItemInfo item_info;
    public int amount;

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        UpdateToolTip();
    }

    //TODO: Extract Durability to extra Class
    public void init(Item item, int current_durability = -1)
    {
        this.item = item;
        this.Size = new Vector2(20, 20);
        this.Position = new Vector2(6, 6);

        Label l = new Label { ZIndex = 1 };
        l.HorizontalAlignment = HorizontalAlignment.Right;
        l.AddThemeFontSizeOverride("font_size", 10);
        l.CustomMinimumSize = new Vector2(30, 32);
        l.Position = new Vector2(-6, -6);
        l.VerticalAlignment = VerticalAlignment.Bottom;
        amount_label = l;
        UpdateToolTip();
        //57cb00
        if (item.item_info.has_durability)
        {
            VBoxContainer vbc = new VBoxContainer();
            vbc.MouseFilter = MouseFilterEnum.Ignore;
            vbc.Size = new Vector2(40, 40);
            vbc.Alignment = BoxContainer.AlignmentMode.End;

            pb = new ProgressBar();
            pb.MouseFilter = MouseFilterEnum.Ignore;
            pb.Size = new Vector2(40, 5);
            pb.MaxValue = item.item_info.max_durability;
            pb.Step = 1;
            pb.Value = current_durability;
            this.current_durability = current_durability;
            pb.ShowPercentage = false;
            StyleBoxFlat sbf = new StyleBoxFlat();
            sbf.BgColor = new Color(0.34f, 0.796f, 0);
            pb.AddThemeStyleboxOverride("fill", sbf);
            vbc.AddChild(pb);
            AddChild(vbc);
        }
        else
            AddChild(l);
    }

    private void UpdateToolTip()
    {
        TooltipText = TranslationServer.Translate(item.item_info.item_name.ToString()) + "\n";
        TooltipText +=
            TranslationServer.Translate(item.item_info.item_description.ToString()) + "\n";
        foreach (ItemType item_type in item.item_info.item_types_arr)
        {
            if (item_type == null)
                continue;

            if (item_type.type == ItemInfo.Type.BURNABLE)
                TooltipText +=
                    TranslationServer.Translate("BURNTIME")
                    + ": "
                    + ((BurnableType)item_type).burntime
                    + "s"
                    + "\n";
            else
                TooltipText +=
                    TranslationServer.Translate("TYPE")
                    + ": "
                    + TranslationServer.Translate(item_type.type.ToString())
                    + "\n";
        }
        foreach (ItemStats stats in item.item_info.item_stats)
            if (stats.bonus > 0)
                TooltipText +=
                    "\n" + TranslationServer.Translate(stats.type.ToString()) + ": +" + stats.bonus;
            else if (stats.bonus < 0)
                TooltipText +=
                    "\n" + TranslationServer.Translate(stats.type.ToString()) + ": " + stats.bonus;
    }

    public void UpdateAmountLabel()
    {
        if (amount_label != null)
            amount_label.Text = item.amount + "x";
    }

    public void SetDurability(int durability)
    {
        pb.Value = durability;
    }

    public override void _Ready()
    {
        Texture = item.item_info.texture;
        ExpandMode = ExpandModeEnum.IgnoreSize;
        StretchMode = StretchModeEnum.KeepAspectCentered;
    }
}
