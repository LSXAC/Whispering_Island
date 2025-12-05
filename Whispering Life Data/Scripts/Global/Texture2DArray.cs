using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Texture2DArray : Node2D
{
    [Export]
    public Array<Texture2D> textures = [];
}
