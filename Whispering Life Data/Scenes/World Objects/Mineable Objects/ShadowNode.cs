using Godot;
using Godot.Collections;

public partial class ShadowNode : Node2D
{
    [Export]
    public int start_index = -1;

    private Sprite2D shadow_sprite;

    [Export]
    public RemoteTransform2D remote_transform;

    [Export]
    public Texture2D single_shadow_texture;

    public override void _Ready()
    {
        shadow_sprite = CreateShadowSprite();
        if (single_shadow_texture != null)
            SetTexture(single_shadow_texture);
    }

    public Sprite2D CreateShadowSprite()
    {
        Sprite2D shadow_sprite = new Sprite2D();
        shadow_sprite.ZIndex = 0;
        shadow_sprite.Centered = true;
        GameManager.instance.shadow_manager.AddChild(shadow_sprite);
        remote_transform.RemotePath = shadow_sprite.GetPath();
        shadow_sprite.GlobalPosition = GlobalPosition;
        return shadow_sprite;
    }

    public void RemoveShadow()
    {
        shadow_sprite.QueueFree();
        remote_transform.RemotePath = null;
    }

    public void SetTexture(Texture2D texture)
    {
        shadow_sprite.Texture = texture;
    }
}
