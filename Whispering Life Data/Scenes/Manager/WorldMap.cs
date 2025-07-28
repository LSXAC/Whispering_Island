using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class WorldMap : CanvasLayer
{
    [Export]
    public Camera2D camera;

    [Export]
    public float max_camera_zoom = 0.3f;

    [Export]
    public float min_camera_zoom = 1f;

    [Export]
    public float speed = 1f;

    [Export]
    public Control icons_parent;

    public static Array<WorldMapIcon> connected_icons = new Array<WorldMapIcon>();
    public PackedScene icon_object = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/icon_object.tscn"
    );

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (GameManager.gameover || GameManager.In_Cutscene)
            return;

        if (Input.IsActionJustPressed("Map"))
        {
            GameMenu.SetWindow(this);
            Player.camera.Enabled = false;
            camera.Enabled = true;
            icons_parent.Visible = true;

            camera.GlobalPosition = IslandManager
                .instance.GetNearestIsland(Player.instance.GlobalPosition)
                .GlobalPosition;

            PlayerUI.instance.Visible = false;
            foreach (Control c in icons_parent.GetChildren())
                c.QueueFree();

            foreach (WorldMapIcon icon in connected_icons)
            {
                if (icon == null || IsInstanceValid(icon) == false)
                    continue;

                IconObject icon_obj = icon_object.Instantiate<IconObject>();

                icon_obj.texture.Texture = icon.icon_texture;
                //icon_obj.Scale = icon.scale;
                icon_obj.GlobalPosition =
                    icon.parent.GlobalPosition
                    - new Vector2(
                        icon.icon_texture.GetWidth() * 3 / 2,
                        icon.icon_texture.GetHeight() * 3 / 2
                    );
                icons_parent.AddChild(icon_obj);
            }
        }
        if (!Visible)
            return;

        if (Input.IsActionJustPressed("Escape"))
        {
            if (GameMenu.IsThisWindow(this))
            {
                Debug.Print("ESCAPE: WORLDMAP");
                Player.camera.Enabled = true;
                camera.Enabled = false;
                icons_parent.Visible = false;
                PlayerUI.instance.Visible = true;
                GameMenu.CloseLastWindow();
            }
        }

        ZoomCamera();
        Movement();
    }

    private void Movement()
    {
        if (Input.IsActionPressed("Up"))
            camera.GlobalPosition += new Vector2(0, -1) * speed;

        if (Input.IsActionPressed("Down"))
            camera.GlobalPosition += new Vector2(0, 1) * speed;

        if (Input.IsActionPressed("Left"))
            camera.GlobalPosition += new Vector2(-1, 0) * speed;

        if (Input.IsActionPressed("Right"))
            camera.GlobalPosition += new Vector2(1, 0) * speed;
    }

    private void ZoomCamera()
    {
        if (Input.IsActionJustReleased("Zoom_In"))
            if (
                (camera.Zoom + new Vector2(0.1f, 0.1f))
                <= new Vector2(min_camera_zoom, min_camera_zoom)
            )
                camera.Zoom += new Vector2(0.1f, 0.1f);

        if (Input.IsActionJustReleased("Zoom_Out"))
            if (
                (camera.Zoom - new Vector2(0.1f, 0.1f))
                >= new Vector2(max_camera_zoom, max_camera_zoom)
            )
                camera.Zoom += new Vector2(-0.1f, -0.1f);
    }
}
