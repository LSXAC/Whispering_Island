using System;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
public partial class Texture2DArray : Node2D
{
    [Export]
    public Vector2[] polygon_points;

    [Export]
    public Vector2 collision_position;

    [Export]
    public Array<Texture2D> textures = [];

    [Export]
    public Array<Texture2D> shadow_textures = [];
}
