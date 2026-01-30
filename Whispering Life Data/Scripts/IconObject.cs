using System;
using Godot;

public partial class IconObject : Panel
{
    [Export]
    public TextureRect texture;

    [Export]
    public WorldMap.WorldMapIconType icon_type = WorldMap.WorldMapIconType.NONE;
}
