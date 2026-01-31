using System;
using System.Runtime.CompilerServices;
using Godot;

public partial class Tutorial : Node2D
{
    [Export]
    public SpriteAnimationManager Tree;

    public static Tutorial instance;

    public override void _Ready()
    {
        instance = this;
        Tree.Visible = false;
    }
}
