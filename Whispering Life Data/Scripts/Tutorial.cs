using System;
using System.Runtime.CompilerServices;
using Godot;

public partial class Tutorial : Node2D
{
    public ShaderMaterial outline_shader = ResourceLoader.Load<ShaderMaterial>(
        "res://Shader Objects/Outline_Shader.tres"
    );

    [Export]
    public Sprite2D Tree;

    [Export]
    public Sprite2D shadow;
    public static Tutorial INSTANCE;

    public override void _Ready()
    {
        INSTANCE = this;
        Tree.Visible = false;
        shadow.Visible = false;
    }
}
