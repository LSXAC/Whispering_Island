using System;
using System.Diagnostics;
using Godot;

public abstract partial class Building_Node : Node2D
{
    [Export]
    public bool ignore_node_structure = false;

    [Export]
    public bool use_wind = false;

    [Export]
    private string title = "";

    [Export]
    private string description = "";

    [Export]
    public bool disable_collision = false;

    [Export]
    public bool show_hover_menu = true;

    public SpriteAnimationManager sprite_anim_manager;

    public CollisionPolygon2D collision_polygon;

    public bool mouse_inside = false;
    public bool is_visible = true;

    private bool _isHolding = false;
    private double _holdTimer = 0.0;
    private const double HOLD_INTERVAL = 0.2;

    public abstract void OnMouseClick();

    public override void _Ready()
    {
        if (ignore_node_structure)
            return;
        sprite_anim_manager = GetNode<SpriteAnimationManager>("SpriteAnimationManager");
        collision_polygon = GetNode<CollisionPolygon2D>("BuildingPolygon");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (
            @event is InputEventMouseButton buttonEvent
            && buttonEvent.ButtonIndex == MouseButton.Left
        )
        {
            if (buttonEvent.Pressed && mouse_inside)
            {
                GetViewport().SetInputAsHandled();
                _isHolding = true;

                OnMouseClick();
            }
            else if (!buttonEvent.Pressed)
            {
                _isHolding = false;
                _holdTimer = 0;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (!_isHolding || !mouse_inside)
            return;

        _holdTimer -= delta;

        if (_holdTimer <= 0)
        {
            OnMouseClick();
            _holdTimer = HOLD_INTERVAL;
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
        if (Logger.NodeIsNotNull(collision_polygon))
            collision_polygon.Disabled = true;
    }
}
