using System;
using Godot;

public partial class WorldMapIcon : Node2D
{
    [Export]
    public Texture2D icon_texture;

    [Export]
    public WorldMap.WorldMapIconType icon_type = WorldMap.WorldMapIconType.NONE;

    [Export]
    public string tooltip_text;

    [Export]
    public Node2D parent;

    [Export]
    public Vector2 scale;

    public override void _Ready()
    {
        WorldMap.connected_icons.Add(this);
    }

    public void RemoveWorldIcon()
    {
        WorldMap.connected_icons.Remove(this);
    }
}
