using System;
using Godot;

public partial class SpriteAnimationManager : Node2D
{
    [Export]
    public SpriteFrames sprite_frames;

    [Export]
    public bool use_sprite = true;

    [Export]
    public bool use_animation_player = false;

    [Export]
    public bool use_animated_sprite = false;

    [Export]
    public string anim_name = "Idle";

    private AnimatedSprite2D animated_sprite;
    private Sprite2D sprite;
    private AnimationPlayer anim_player;
    private string current_animation = "";

    public ShadowNode shadowNode;

    public override void _Ready()
    {
        if (Logger.NodeIsNotNull(GetNode<ShadowNode>("ShadowNode")))
            shadowNode = GetNode<ShadowNode>("ShadowNode");

        if (use_sprite)
        {
            sprite = GetNode<Sprite2D>("Sprite2D");
        }

        if (use_animation_player)
        {
            anim_player = GetNode<AnimationPlayer>("AnimationPlayer");
            anim_player.Play(anim_name);
        }

        if (use_animated_sprite)
        {
            animated_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            animated_sprite.Play(anim_name);
        }
    }

    public new Vector2 GetGlobalPosition()
    {
        if (IsAnimated())
            return animated_sprite.GlobalPosition;
        if (sprite != null)
            return sprite.GlobalPosition;
        return Vector2.Zero;
    }

    public void MakeTransparent()
    {
        GetCanvasItem()?.Set("self_modulate", new Color(1f, 1f, 1f, 0.75f));
    }

    public Texture2D GetTexture2D()
    {
        if (IsAnimated())
            return animated_sprite.SpriteFrames.GetFrameTexture(current_animation, 0);
        if (sprite != null)
            return sprite.Texture;
        return null;
    }

    public void SetTexture2D(Texture2D texture)
    {
        if (IsAnimated())
            return;
        if (sprite != null)
            sprite.Texture = texture;
    }

    public new CanvasItem GetCanvasItem()
    {
        if (IsAnimated())
            return animated_sprite;
        if (sprite != null)
            return sprite;
        return null;
    }

    private bool IsAnimated()
    {
        return use_animated_sprite || use_animation_player;
    }
}
