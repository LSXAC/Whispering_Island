using System;
using System.Diagnostics;
using Godot;

public partial class h_box_item : VBoxContainer
{
    public enum colorType
    {
        red,
        white,
        green
    };

    Color red = new Color(1, 0, 0, 1);
    Color white = new Color(1, 1, 1, 1);
    Color green = new Color(0, 1, 0, 1);

    public Label item_label,
        title_label;

    public TextureRect item_texture;

    public override void _Ready()
    {
        item_label = GetNode<Label>("ItemLabel");
        title_label = GetNode<Label>("ItemLabelTitle");
        item_texture = GetNode<TextureRect>("ItemTexture");
    }

    public void InitItemUI(Item item, bool with_name = false)
    {
        if (item_label == null)
            item_label = GetNode<Label>("ItemLabel");
        if (item_texture == null)
            item_texture = GetNode<TextureRect>("ItemTexture");
        if (title_label == null)
            title_label = GetNode<Label>("ItemLabelTitle");

        if (with_name)
        {
            title_label.Visible = true;
            item_label.Text =
                TranslationServer.Translate("CRAFTING_MENU_AMOUNT")
                + ": "
                + ((int)(item.amount * GameManager.difficulty_multiplier))
                + "x";
            title_label.Text = TranslationServer.Translate(item.info.name);
        }
        else
            item_label.Text = " " + ((int)(item.amount * GameManager.difficulty_multiplier)) + "x";
        item_texture.Texture = item.info.texture;
        item_texture.TooltipText = Inventory.GetToolTipItem(item);
    }

    public void ChangeColor(colorType type)
    {
        if (type == colorType.red)
            item_label.Modulate = red;
        else if (type == colorType.white)
            item_label.Modulate = white;
        else
            item_label.Modulate = green;
    }

    public bool CanCraftItem(Item need, int amount)
    {
        ChangeColor(colorType.red);
        if (need != null)
        {
            if (need.amount >= amount)
            {
                ChangeColor(colorType.white);
                return true;
            }
        }
        return false;
    }
}
