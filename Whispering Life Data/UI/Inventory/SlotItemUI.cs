using System;
using System.Diagnostics;
using Godot;

public partial class SlotItemUI : Control
{
    public Label amount_label;

    public ProgressBar progress_bar;
    public int max_durability = 0;
    public int current_durability = -1;

    [Export]
    public Item item;

    [Export]
    public TextureRect item_texture_rect;

    [Export]
    public Control poison_icon;

    private CustomToolTip custom_tooltip;
    private PackedScene custom_tool_tip_scene = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://drhdbuo05k0tx")
    );

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        UpdateToolTip();
    }

    public void init(Item item, int current_durability = -1)
    {
        this.item = item.Clone();
        item_texture_rect.Texture = item.info.texture;
        amount_label.Text = item.amount + "x";
        UpdateToolTip();
        UpdatePoisonIcon();
        Debug.Print("Init Slot Item UI!");

        WearableAttribute attribute = item.info.GetAttributeOrNull<WearableAttribute>();
        //57cb00
        if (attribute == null)
        {
            amount_label.Visible = true;
            progress_bar.Visible = false;
            return;
        }
        max_durability = (int)(
            attribute.durability
            * Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.TOOL_DURABILITY)
        );
        VBoxContainer vbc = new VBoxContainer();
        vbc.MouseFilter = MouseFilterEnum.Ignore;
        vbc.Size = new Vector2(40, 40);
        vbc.Alignment = BoxContainer.AlignmentMode.End;

        //Durability Bar
        progress_bar.MaxValue = max_durability;
        progress_bar.Value = current_durability;
        this.current_durability = current_durability;

        StyleBoxFlat sbf = new StyleBoxFlat();
        sbf.BgColor = new Color(0.34f, 0.796f, 0);
        progress_bar.AddThemeStyleboxOverride("fill", sbf);
        UpdateToolTip();
    }

    public void RescaleDurabilityBar()
    {
        WearableAttribute attribute = item.info.GetAttributeOrNull<WearableAttribute>();
        //57cb00
        if (attribute != null)
        {
            max_durability = (int)(
                attribute.durability
                * Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.TOOL_DURABILITY)
            );
            progress_bar.MaxValue = max_durability;
        }
    }

    public void UpdateToolTip()
    {
        if (item?.info == null || custom_tooltip == null)
            return;

        custom_tooltip.Update(item, current_durability, max_durability);
    }

    public void UpdateAmountLabel()
    {
        if (amount_label != null)
            amount_label.Text = item.amount + "x";
        UpdatePoisonIcon();
    }

    private void UpdatePoisonIcon()
    {
        if (poison_icon != null && item != null)
        {
            poison_icon.Visible = item.state == Item.STATE.POISONED;
        }
    }

    public void SetDurability(int durability)
    {
        progress_bar.Value = durability;
    }

    public override void _Ready()
    {
        amount_label = GetNode<Label>("Label");
        progress_bar = GetNode<VBoxContainer>("VBoxContainer").GetNode<ProgressBar>("ProgressBar");
        amount_label.Visible = false;

        if (poison_icon != null)
            poison_icon.Visible = false;

        custom_tooltip = custom_tool_tip_scene.Instantiate<CustomToolTip>();
        AddChild(custom_tooltip);
    }
}
