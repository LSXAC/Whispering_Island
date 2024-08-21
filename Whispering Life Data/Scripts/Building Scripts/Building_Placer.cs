using System;
using System.Diagnostics;
using Godot;

public partial class Building_Placer : Node2D
{
    [Export]
    public Node2D parent_Node;

    [Export]
    public TileMap tilemap;
    public static Node2D current_building = null;
    private placeable_building placeable;
    private Belt belt;
    private int current_belt_rotation = 3;
    public static PackedScene building = null;

    public void InitBuilding(PackedScene scene)
    {
        if (scene == null)
        {
            GD.PrintErr("Building is Null, can not be initialised");
            Building_Menu.instance.Visible = false;
            Game_Manager.in_building_mode = false;
            return;
        }

        current_building = (Node2D)scene.Instantiate();

        building = scene;
        parent_Node.AddChild(current_building);
        if (current_building is Area2D)
        {
            belt = current_building as Belt;
            belt.Set_Rotation(current_belt_rotation);
            belt.Monitorable = false;
            return;
        }
        if (current_building is placeable_building)
        {
            placeable = current_building as placeable_building;
            placeable.sprite.SelfModulate = new Color(1f, 1f, 1f, 0.0f);
            if (placeable.building_content != null)
                placeable.building_content.Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        if (current_building == null)
            return;

        Vector2 pos = tilemap.LocalToMap(GetGlobalMousePosition());
        current_building.Position = new Vector2(pos.X * 16, pos.Y * 16);

        if (belt != null)
        {
            if (Input.IsActionJustPressed("Rotate_Right"))
            {
                current_belt_rotation++;
                if (current_belt_rotation == 4)
                    current_belt_rotation = 0;
                belt.Set_Rotation(current_belt_rotation);
            }
            if (Input.IsActionJustPressed("Rotate_Left"))
            {
                current_belt_rotation--;
                if (current_belt_rotation == -1)
                    current_belt_rotation = 3;
                belt.Set_Rotation(current_belt_rotation);
            }
            if (Input.IsActionJustPressed("Close"))
                CloseMenu();
        }

        if (Input.IsActionJustPressed("MouseLeft"))
        {
            Debug.Print("Mouse Pressed");
            if (placeable != null)
            {
                Debug.Print("placeable not null");
                if (placeable.building_collider_manager.AllCollidersOnBuildingLayer())
                {
                    BuildBuilding();
                    Debug.Print("Build!");
                }
            }
            if (belt != null)
            {
                if (belt.building_collider_manager.AllCollidersOnBuildingLayer())
                {
                    BuildBuilding();
                    Debug.Print("Build!");
                }
            }
        }
    }

    public void BuildBuilding()
    {
        Node2D temp = (Node2D)building.Instantiate();
        Vector2 pos = tilemap.LocalToMap(GetGlobalMousePosition());
        temp.Position = new Vector2(pos.X * 16, pos.Y * 16);
        parent_Node.AddChild(temp);

        if (temp is Belt)
        {
            //GetTree().CallGroup("Belt", "OnItemHolderItemHeld");
            ((Belt)temp).Set_Rotation(current_belt_rotation);
            return;
        }
        CloseMenu();
    }

    public void CloseMenu()
    {
        Node2D tempB = current_building;
        current_building = null;
        tempB.QueueFree();
        Game_Manager.in_building_mode = false;
        placeable = null;
        belt = null;
    }
}
