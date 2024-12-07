using System;
using System.Diagnostics;
using Godot;

public partial class h_box_item : HBoxContainer
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

    public Label item_label;

    public TextureRect item_texture;

    public override void _Ready()
    {
        item_label = GetNode<Label>("ItemLabel");
        item_texture = GetNode<VBoxContainer>("VBoxContainer").GetNode<TextureRect>("ItemTexture");
    }

    public void InitItemUI(string item_name, int amount, Texture2D texture)
    {
        if (item_label == null)
            item_label = GetNode<Label>("ItemLabel");
        if (item_texture == null)
            item_texture = GetNode<VBoxContainer>("VBoxContainer")
                .GetNode<TextureRect>("ItemTexture");

        item_label.Text = TranslationServer.Translate(item_name) + " " + amount + "x";
        item_texture.Texture = texture;
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
