using System;
using Godot;

public partial class HitLabelManager : Control
{
    private PackedScene hit_label = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://d1l2gqiubblcd")
    );

    public override void _Ready()
    {
        Size = Vector2.Zero;
    }

    public void ShowHitLabel(int miningAmount)
    {
        CharacterBody2D hit_lab = hit_label.Instantiate() as CharacterBody2D;
        hit_lab.GetChild<HitLabel>(0).Init(miningAmount, this);
        GameManager.instance.AddChild(hit_lab);
    }
}
