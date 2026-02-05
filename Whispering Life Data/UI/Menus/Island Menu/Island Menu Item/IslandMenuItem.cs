using System;
using Godot;

public partial class IslandMenuItem : ColorRect
{
    [Export]
    public ItemMenuItemData item_menu_item_data;

    [Export]
    public TextureRect image;

    [Export]
    public Button buy_btn;

    [Export]
    public Label money_label;

    public override void _Ready()
    {
        image.Texture = item_menu_item_data.island_texture;
        buy_btn.Text = TranslationServer.Translate(item_menu_item_data.menu_item_title);
    }

    public void UpdateMoneyLabel(string text)
    {
        money_label.Text = text;
    }
}
