using System;
using System.Numerics;
using Godot;

public partial class Player : CharacterBody2D
{
    public static Player INSTANCE = null;
    public AnimatedSprite2D anim;
    private Player_Stamina player_stamina;
    private Camera2D cam;
    private string current_direction = "Down";
    private float velo_x = 0,
        velo_y = 0;
    private float max_zoom_offset = 2.2f;

    public override void _Ready()
    {
        INSTANCE = this;
        player_stamina = GetNode<Player_Stamina>("Player_Stamina");
        anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        cam = GetNode<Camera2D>("Camera2D");

        cam.Zoom = new Godot.Vector2(1.5f, 1.5f);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (
            Game_Manager.inside_game_menu
            || Game_Manager.building_mode != Game_Manager.BuildingMode.None
        )
            return;

        if (Input.IsActionJustReleased("Zoom_In"))
            if (
                (cam.Zoom + new Godot.Vector2(0.15f, 0.15f))
                <= new Godot.Vector2(max_zoom_offset, max_zoom_offset)
            )
                cam.Zoom += new Godot.Vector2(0.15f, 0.15f);

        if (Input.IsActionJustReleased("Zoom_Out"))
            if ((cam.Zoom - new Godot.Vector2(0.15f, 0.15f)) >= new Godot.Vector2(0.8f, 0.8f))
                cam.Zoom += new Godot.Vector2(-0.15f, -0.15f);

        this.velo_x = 0;
        this.velo_y = 0;

        if (Input.IsActionPressed("Up"))
            velo_y = -1;

        if (Input.IsActionPressed("Down"))
            velo_y = 1;

        if (Input.IsActionPressed("Left"))
            velo_x = -1;

        if (Input.IsActionPressed("Right"))
            velo_x = 1;

        if (this.Velocity == Godot.Vector2.Zero)
        {
            if (current_direction == "Down")
                anim.Play("Idle_Down");
            if (current_direction == "Up")
                anim.Play("Idle_Up");
            if (current_direction == "Left")
                anim.Play("Idle_Left");
            if (current_direction == "Right")
                anim.Play("Idle_Right");
        }
        this.Velocity = new Godot.Vector2(this.velo_x, this.velo_y);
        player_stamina.UpdateStaminaDependencies(velo_x, velo_y);
        anim.Play(GetDirection());
        MoveAndSlide();
    }

    private string GetDirection()
    {
        bool isUp = this.velo_y < 0;
        bool isDown = this.velo_y > 0;
        bool isLeft = this.velo_x < 0;
        bool isRight = this.velo_x > 0;

        if (isUp && current_direction == "Up")
            return current_direction;

        if (isDown && current_direction == "Down")
            return current_direction;

        if (isLeft && current_direction == "Left")
            return current_direction;

        if (isRight && current_direction == "Right")
            return current_direction;

        if (isUp)
            current_direction = "Up";
        if (isDown)
            current_direction = "Down";
        if (isLeft)
            current_direction = "Left";
        if (isRight)
            current_direction = "Right";

        return null;
    }
}
