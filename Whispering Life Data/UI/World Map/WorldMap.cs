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

    [Export]
    public CheckBox quest_checkbox,
        traders_checkbox,
        removeable_checkbox;

    public enum WorldMapIconType
    {
        NONE,
        REMOVEABLE,
        TRADERS,
        QUESTS,
    }

    public static Array<WorldMapIcon> connected_icons = new Array<WorldMapIcon>();
    public PackedScene icon_object = ResourceLoader.Load<PackedScene>(
        ResourceUid.UidToPath("uid://cca4fqqi7u7tj")
    );

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (GameManager.gameover || CutsceneManager.In_Cutscene)
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
                icon_obj.icon_type = icon.icon_type;
                //icon_obj.Scale = icon.scale;
                icon_obj.GlobalPosition = icon.parent.GlobalPosition - new Vector2(32, 32);
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

    public void OnClearFilters()
    {
        foreach (IconObject icon in icons_parent.GetChildren())
        {
            icon.Visible = true;
        }
        quest_checkbox.ButtonPressed = true;
        traders_checkbox.ButtonPressed = true;
        removeable_checkbox.ButtonPressed = true;
    }

    public void OnCheckBoxQuestToggled(bool btn_ticked)
    {
        foreach (IconObject icon in icons_parent.GetChildren())
        {
            if (icon.icon_type == WorldMapIconType.QUESTS)
            {
                icon.Visible = btn_ticked;
            }
        }
    }

    public void OnCheckBoxRemoveableToggled(bool btn_ticked)
    {
        foreach (IconObject icon in icons_parent.GetChildren())
        {
            if (icon.icon_type == WorldMapIconType.REMOVEABLE)
            {
                icon.Visible = btn_ticked;
            }
        }
    }

    public void OnCheckBoxTradersToggled(bool btn_ticked)
    {
        foreach (IconObject icon in icons_parent.GetChildren())
        {
            if (icon.icon_type == WorldMapIconType.TRADERS)
            {
                icon.Visible = btn_ticked;
            }
        }
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
