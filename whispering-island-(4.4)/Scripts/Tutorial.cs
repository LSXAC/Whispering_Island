using System;
using System.Runtime.CompilerServices;
using Godot;

public partial class Tutorial : Node2D
{
    public ShaderMaterial outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Shader/Outline_Shader.tres"
    );

    [Export]
    public SpriteAnimationManager Tree;

    public static Tutorial instance;

    public override void _Ready()
    {
        instance = this;
        Tree.Visible = false;
    }
}
