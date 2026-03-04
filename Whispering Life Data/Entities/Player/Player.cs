using System;
using Godot;

public partial class Player : CharacterBody2D
{
    [Export]
    public Texture2D shadow_texture;
    public static Player instance = null;
    public static Camera2D camera;
    public static CharacterSave char_save = new CharacterSave();
    public AnimatedSprite2D anim;
    public PlayerStats player_stats;
    private PlayerStamina player_stamina;
    private string current_direction = "Up";
    private Vector2 velo = Vector2.Zero;

    private float max_zoom_offset = 2.5f;
    private float min_zoom_offset = 1f;
    private float normal_zoom_offset = 1.5f;
    private float zoom_speed = 0.1f;
    private ShadowNode shadowNode;

    public override void _Ready()
    {
        anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        if (Logger.NodeIsNotNull(GetNode<ShadowNode>("ShadowNode")))
        {
            shadowNode = GetNode<ShadowNode>("ShadowNode");
            shadowNode.SetTexture(shadow_texture);
        }

        instance = this;
        player_stamina = GetNode<PlayerStamina>("PlayerStamina");
        player_stats = GetNode<PlayerStats>("PlayerStats");

        camera = GetNode<Camera2D>("Camera2D");
        camera.Zoom = new Vector2(normal_zoom_offset, normal_zoom_offset);
    }

    public override void _PhysicsProcess(double delta)
    {
        ChoosePlayerAnimation();

        if (GameManager.gameover)
        {
            Velocity = Vector2.Zero;
            return;
        }

        player_stamina.RegenerateStamina(this.velo);

        if (CutsceneManager.In_Cutscene)
        {
            Velocity = Vector2.Zero;
            return;
        }

        if (GameMenu.IsWindowActiv() && !GameMenu.IsThisWindow(BuildMenu.instance))
            return;

        if (
            (
                GameMenu.IsThisWindow(BuildMenu.instance)
                && GameManager.building_mode == GameManager.BuildingMode.Placing
            ) || !GameMenu.IsWindowActiv()
        )
            ZoomCamera();

        this.velo = Vector2.Zero;

        if (Input.IsActionPressed("Up"))
            velo = new Vector2(velo.X, -1);

        if (Input.IsActionPressed("Down"))
            velo = new Vector2(velo.X, 1);

        if (Input.IsActionPressed("Left"))
            velo = new Vector2(-1, velo.Y);

        if (Input.IsActionPressed("Right"))
            velo = new Vector2(1, velo.Y);

        if (this.Velocity == Vector2.Zero)
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
        this.Velocity = velo;
        player_stamina.UpdateStaminaDependencies(velo);
        ChoosePlayerAnimation();
        MoveAndSlide();
    }

    private void ChoosePlayerAnimation()
    {
        if (this.Velocity == Vector2.Zero)
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
        anim.Play(GetDirection());
    }

    private void ZoomCamera()
    {
        if (Input.IsActionJustReleased("Zoom_In"))
            if (
                (camera.Zoom + new Vector2(zoom_speed, zoom_speed))
                <= new Vector2(max_zoom_offset, max_zoom_offset)
            )
                camera.Zoom += new Vector2(zoom_speed, zoom_speed);

        if (Input.IsActionJustReleased("Zoom_Out"))
            if (
                (camera.Zoom - new Vector2(zoom_speed, zoom_speed))
                >= new Vector2(min_zoom_offset, min_zoom_offset)
            )
                camera.Zoom += new Vector2(-zoom_speed, -zoom_speed);
    }

    private string GetDirection()
    {
        bool isUp = this.velo.Y < 0;
        bool isDown = this.velo.Y > 0;
        bool isLeft = this.velo.X < 0;
        bool isRight = this.velo.X > 0;

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
