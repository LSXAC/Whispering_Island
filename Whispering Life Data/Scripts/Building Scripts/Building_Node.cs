using System;
using System.Diagnostics;
using Godot;

public abstract partial class Building_Node : Node2D
{
    [Export]
    public bool ignore_node_structure = false;

    [Export]
    private string title = "";

    [Export]
    private string description = "";

    [Export]
    public bool disable_collision = false;

    public SpriteAnimationManager sprite_anim_manager;

    public CollisionShape2D collision_shape;

    public bool mouse_inside = false;

    public abstract void OnMouseClick();

    public override void _Ready()
    {
        if (ignore_node_structure)
            return;
        sprite_anim_manager = GetNode<SpriteAnimationManager>("SpriteAnimationManager");
        collision_shape = GetNode<CollisionShape2D>("BuildingCollision");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton buttonevent)
            if (buttonevent.ButtonIndex == MouseButton.Left && buttonevent.Pressed && mouse_inside)
            {
                OnMouseClick();
            }
    }

    public Texture2D GetTextureFromSpriteManager()
    {
        if (Logger.NodeIsNull(sprite_anim_manager))
            return null;

        return sprite_anim_manager.GetTexture2D();
    }

    public void SetTextureToSpriteManager(Texture2D texture)
    {
        if (Logger.NodeIsNull(sprite_anim_manager))
            return;

        sprite_anim_manager.SetTexture2D(texture);
    }

    public new Vector2 GetGlobalPosition()
    {
        if (Logger.NodeIsNotNull(sprite_anim_manager))
            return sprite_anim_manager.GetGlobalPosition();
        return new Vector2(0, 0);
    }

    public string GetTitle()
    {
        if (title.Length == 0)
            Debug.Print("Title is Empty in: ", this);
        return title;
    }

    public string GetDescription()
    {
        if (description.Length == 0)
            Debug.Print("Description is Empty in: ", this);
        return description;
    }

    public Vector2 GetBuildingPosition()
    {
        Vector2 vec = GetGlobalPosition();
        return new Vector2(vec.X, vec.Y - 40f);
    }

    public void DisableCollision()
    {
        if (Logger.NodeIsNotNull(collision_shape))
            collision_shape.Disabled = true;
    }
}
