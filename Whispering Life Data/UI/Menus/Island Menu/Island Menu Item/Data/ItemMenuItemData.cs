using System;
using Godot;

[GlobalClass]
public partial class ItemMenuItemData : Resource
{
    [Export]
    public Texture2D island_texture;

    [Export]
    public string menu_item_title;

    [Export]
    public PackedScene island_scene;
}
