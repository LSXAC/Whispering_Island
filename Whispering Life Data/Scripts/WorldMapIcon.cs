using System;
using Godot;

public partial class WorldMapIcon : Node2D
{
    [Export]
    public Texture2D icon_texture;

    [Export]
    public string tooltip_text;

    [Export]
    public Node2D parent;

    public override void _Ready()
    {
        TestMap.connected_icons.Add(this);
    }

    public void RemoveWorldIcon()
    {
        TestMap.connected_icons.Remove(this);
    }
}
