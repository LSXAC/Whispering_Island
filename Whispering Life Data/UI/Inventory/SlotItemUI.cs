using System;
using System.Diagnostics;
using Godot;

public partial class SlotItemUI : TextureRect
{
    public Label amount_label;

    public ProgressBar progress_bar;
    public int max_durability = 0;
    public int current_durability = -1;

    [Export]
    public Item item;

    public override void _Notification(int what)
    {
        if (what != NotificationTranslationChanged)
            return;

        UpdateToolTip();
    }

    //TODO: Extract Durability to extra Class
    public void init(Item item, int current_durability = -1)
    {
        this.item = item.Clone();
        Texture = item.info.texture;
        amount_label.Text = item.amount + "x";
        UpdateToolTip();
        Debug.Print("Update ToolTip!");

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
        if (item?.info == null)
            return;
        TooltipText = TranslationServer.Translate(item.info.name.ToString()) + "\n\n";
        TooltipText += TranslationServer.Translate(item.info.description.ToString()) + "\n\n";
        if (current_durability != -1)
            TooltipText += "Durability: " + current_durability + "/" + max_durability;
        TooltipText += "\n";
        foreach (ItemAttributeBase item_attribute in item.info.attributes)
        {
            if (item_attribute == null)
                continue;

            TooltipText += item_attribute.GetNameOfAttribute();
        }
        TooltipText += "\n";
        TooltipText += "State: " + item.state.ToString();
    }

    public void UpdateAmountLabel()
    {
        if (amount_label != null)
            amount_label.Text = item.amount + "x";
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
    }
}
