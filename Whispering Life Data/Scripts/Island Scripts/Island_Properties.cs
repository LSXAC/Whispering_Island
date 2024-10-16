using System;
using System.Runtime.ExceptionServices;
using Godot;

public partial class Island_Properties : Node2D
{
    [Export]
    public int unique_island_id = -1;

    [Export]
    public int matrix_island_id = 0;

    [Export]
    private int island_tiles = 16;

    [Export]
    private bool up_closed = false;

    [Export]
    private bool right_closed = false;

    [Export]
    private bool down_closed = false;

    [Export]
    private bool left_closed = false;

    public ObjectSpawnerTilemap ost;

    private Node2D bridge_start_points;
    private PackedScene BRIDGE_SIDE = ResourceLoader.Load<PackedScene>(
        "res://Sprites/Bridge/bridge_side.tscn"
    );
    private PackedScene BRIDGE_SIDE_END = ResourceLoader.Load<PackedScene>(
        "res://Sprites/Bridge/bridge_side_end.tscn"
    );
    private PackedScene BRIDGE_UP = ResourceLoader.Load<PackedScene>(
        "res://Sprites/Bridge/bridge_up.tscn"
    );
    private PackedScene BRIDGE_UP_END = ResourceLoader.Load<PackedScene>(
        "res://Sprites/Bridge/bridge_up_end.tscn"
    );
    private PackedScene BRIDGE_BOTTOM_END = ResourceLoader.Load<PackedScene>(
        "res://Sprites/Bridge/bridge_bottom_end.tscn"
    );
    private Node2D bridges_parent;
    private Node2D signs_parent;
    private StaticBody2D bridge_collision_parent;
    private const byte bridge_size_width = 32;

    public enum DIRECTION
    {
        NONE,
        UP,
        RIGHT,
        LEFT,
        DOWN
    };

    [Export]
    public int matrix_x = 0;

    [Export]
    public int matrix_y = 0;

    private float x = 0;
    private float y = 0;

    public override void _Ready()
    {
        ost = GetNode<ObjectSpawnerTilemap>("ObjectSpawnerTilemap");
        ost.roms.matrix_island_id = matrix_island_id;
        Node2D start_node = GetNode<Node2D>("IslandProperties");
        bridge_start_points = start_node.GetNode<Node2D>("BridgeStartPoints");
        bridge_collision_parent = start_node.GetNode<StaticBody2D>("BridgeCollisions");
        bridges_parent = start_node.GetNode<Node2D>("Bridges");
        signs_parent = start_node.GetNode<Node2D>("Signs");
    }

    public void GetSigns()
    {
        //Get Signs und Set next_ip (from instantiated Island) = current_ip;
        //Also disable Signs, when from X <- Y | X (Right), Y (Left) Depending on Matrix
        Node2D node = null;
        if (
            (Game_Manager.IsIslandOnMatrix(matrix_x, matrix_y - 1) || up_closed)
            && signs_parent.HasNode("UpSign")
        )
        {
            node = signs_parent.GetNode<Node2D>("UpSign");
            bridge_collision_parent.GetNode<CollisionShape2D>("Up").Disabled = true;
        }

        if (
            (Game_Manager.IsIslandOnMatrix(matrix_x, matrix_y + 1) || down_closed)
            && signs_parent.HasNode("DownSign")
        )
        {
            node = signs_parent.GetNode<Node2D>("DownSign");
            bridge_collision_parent.GetNode<CollisionShape2D>("Down").Disabled = true;
        }

        if (
            (Game_Manager.IsIslandOnMatrix(matrix_x + 1, matrix_y) || right_closed)
            && signs_parent.HasNode("RightSign")
        )
        {
            node = signs_parent.GetNode<Node2D>("RightSign");
            bridge_collision_parent.GetNode<CollisionShape2D>("Right").Disabled = true;
        }

        if (
            (Game_Manager.IsIslandOnMatrix(matrix_x - 1, matrix_y) || left_closed)
            && signs_parent.HasNode("LeftSign")
        )
        {
            node = signs_parent.GetNode<Node2D>("LeftSign");
            bridge_collision_parent.GetNode<CollisionShape2D>("Left").Disabled = true;
        }
        if (node == null)
            return;
        node.QueueFree();
    }

    public void CreateAnotherIsland(
        PackedScene island_prefab,
        DIRECTION dir,
        bool is_loading = false
    )
    {
        GD.Print("Step 1: Initialize");
        Vector2 new_pos = Vector2.Zero;

        Node2D island = (Node2D)island_prefab.Instantiate();
        Island_Properties ip = island as Island_Properties;
        Game_Manager.INSTANCE.island_parent.AddChild(island);

        GD.Print("Step 2: Select Direction");
        switch (dir)
        {
            case DIRECTION.UP:
                ip.matrix_y = matrix_y - 1;
                ip.matrix_x = matrix_x;
                Game_Manager.SetIslandOnMatrix(matrix_x, matrix_y - 1, true);
                up_closed = true;
                ip.down_closed = true;
                SetPositionToNewIsland(island, ip);
                SetBridges(bridge_start_points.GetNode<Node2D>("Up").Position, DIRECTION.UP, ip);

                GD.Print("Step 2.1: Up");
                break;
            case DIRECTION.RIGHT:
                ip.matrix_x = matrix_x + 1;
                ip.matrix_y = matrix_y;
                Game_Manager.SetIslandOnMatrix(matrix_x + 1, matrix_y, true);
                right_closed = true;
                ip.left_closed = true;
                SetPositionToNewIsland(island, ip);
                SetBridges(
                    bridge_start_points.GetNode<Node2D>("Right").Position,
                    DIRECTION.RIGHT,
                    ip
                );

                GD.Print("Step 2.1: Right");
                break;
            case DIRECTION.LEFT:
                ip.matrix_x = matrix_x - 1;
                ip.matrix_y = matrix_y;
                Game_Manager.SetIslandOnMatrix(matrix_x - 1, matrix_y, true);
                left_closed = true;
                ip.right_closed = true;
                SetPositionToNewIsland(island, ip);
                SetBridges(
                    bridge_start_points.GetNode<Node2D>("Left").Position,
                    DIRECTION.LEFT,
                    ip
                );

                GD.Print("Step 2.1: Left");
                break;
            case DIRECTION.DOWN:
                ip.matrix_y = matrix_y + 1;
                ip.matrix_x = matrix_x;
                Game_Manager.SetIslandOnMatrix(matrix_x, matrix_y + 1, true);
                //x = GetParent<Node2D>().Position.X+bridge_start_points.GetNode<Node2D>("Down").Position.X;
                //y = bridge_start_points.GetNode<Node2D>("Down").Position.Y+GetParent<Node2D>().Position.Y+bridge_parts_amount*bridge_size_width;
                SetPositionToNewIsland(island, ip);
                SetBridges(
                    bridge_start_points.GetNode<Node2D>("Down").Position,
                    DIRECTION.DOWN,
                    ip
                );
                down_closed = true;
                ip.up_closed = true;

                GD.Print("Step 2.1: Down");
                break;

            default:
                GD.Print("NO DIRECTION");
                break;
        }
        if (!is_loading)
            Islands_Manager.INSTANCE.SaveIsland(dir, matrix_island_id, ip.unique_island_id);
        Islands_Manager.INSTANCE.last_island_id += 1;
        ip.matrix_island_id = Islands_Manager.INSTANCE.last_island_id;
        ip.ost.roms.matrix_island_id = ip.matrix_island_id;
        island_menu.instance.current_sign = null;
        GetSigns();
        ip.GetSigns();
        //ip.ReplaceTiles();
    }

    private void SetPositionToNewIsland(Node2D island, Island_Properties ip)
    {
        x = ip.matrix_x * 16 * 32;
        y = ip.matrix_y * 16 * 32;
        island.Position = new Vector2(x, y);
    }

    /*public void ReplaceTiles()
    {
        for(int x = -8; x<9; x++)
            for(int y = -8; y<9;y++){
                outside_tilemap.SetCell(0,outside_tilemap.LocalToMap(new Vector2(Position.X+x*32+matrix_x * 16*32,Position.Y+y*32+matrix_y * 16*32)),-1);
            }
    }*/

    private void SetBridges(Vector2 start, DIRECTION dir, Island_Properties ip_t)
    {
        Node2D bridge_part = null;

        float x = 0;
        Vector2 temp = new Vector2();
        switch (dir) // 0=UP, 1= RIGHT, 2 =LEFT, 3=DOWN
        {
            case DIRECTION.UP:
                temp =
                    ip_t.bridge_start_points.GetNode<Node2D>("Down").GlobalPosition
                    - bridge_start_points.GetNode<Node2D>("Up").GlobalPosition;
                x = Math.Abs(temp.Y / 32);
                GD.Print(x);
                for (int i = 0; i <= x; i++)
                {
                    bridge_part = (Node2D)BRIDGE_UP.Instantiate();
                    bridge_part.Position = new Vector2(start.X, start.Y - bridge_size_width * i);
                    bridges_parent.AddChild(bridge_part);
                }

                //Bridge Start and End Tile
                bridge_part = (Node2D)BRIDGE_BOTTOM_END.Instantiate();
                bridge_part.Position = new Vector2(start.X, start.Y - bridge_size_width * -1);
                bridges_parent.AddChild(bridge_part);

                bridge_part = (Node2D)BRIDGE_UP_END.Instantiate();
                bridge_part.Position = new Vector2(start.X, start.Y - bridge_size_width * (x + 1));
                bridge_part.Scale = new Vector2(-1f, 1f);
                bridges_parent.AddChild(bridge_part);
                break;
            case DIRECTION.RIGHT:
                temp =
                    ip_t.bridge_start_points.GetNode<Node2D>("Left").GlobalPosition
                    - bridge_start_points.GetNode<Node2D>("Right").GlobalPosition;
                x = Math.Abs(temp.X / 32);
                GD.Print(ip_t.bridge_start_points.GetNode<Node2D>("Right").Position.X);
                GD.Print(bridge_start_points.GetNode<Node2D>("Left").Position.X);
                GD.Print(x);

                for (int i = 0; i <= x; i++)
                {
                    bridge_part = (Node2D)BRIDGE_SIDE.Instantiate();
                    bridge_part.Position = new Vector2(start.X + bridge_size_width * i, start.Y);
                    bridges_parent.AddChild(bridge_part);
                }
                //Bridge Start and End Tile
                bridge_part = (Node2D)BRIDGE_SIDE_END.Instantiate();
                bridge_part.Position = new Vector2(start.X + bridge_size_width * -1, start.Y);
                bridges_parent.AddChild(bridge_part);

                bridge_part = (Node2D)BRIDGE_SIDE_END.Instantiate();
                bridge_part.Position = new Vector2(start.X + bridge_size_width * (x + 1), start.Y);
                bridge_part.Scale = new Vector2(-1f, 1f);
                bridges_parent.AddChild(bridge_part);

                break;
            case DIRECTION.LEFT:
                temp =
                    ip_t.bridge_start_points.GetNode<Node2D>("Right").GlobalPosition
                    - bridge_start_points.GetNode<Node2D>("Left").GlobalPosition;
                x = Math.Abs(temp.X / 32);
                GD.Print(x);
                for (int i = 0; i <= x; i++)
                {
                    bridge_part = (Node2D)BRIDGE_SIDE.Instantiate();
                    bridge_part.Position = new Vector2(start.X - bridge_size_width * i, start.Y);
                    bridges_parent.AddChild(bridge_part);
                }

                //Bridge Start and End Tile
                bridge_part = (Node2D)BRIDGE_SIDE_END.Instantiate();
                bridge_part.Position = new Vector2(start.X - bridge_size_width * -1, start.Y);
                bridge_part.Scale = new Vector2(-1f, 1f);
                bridges_parent.AddChild(bridge_part);

                bridge_part = (Node2D)BRIDGE_SIDE_END.Instantiate();
                bridge_part.Position = new Vector2(start.X - bridge_size_width * (x + 1), start.Y);
                bridges_parent.AddChild(bridge_part);

                break;
            case DIRECTION.DOWN:
                temp =
                    ip_t.bridge_start_points.GetNode<Node2D>("Up").GlobalPosition
                    - bridge_start_points.GetNode<Node2D>("Down").GlobalPosition;
                x = Math.Abs(temp.Y / 32);
                GD.Print(x);
                for (int i = 0; i <= x; i++)
                {
                    bridge_part = (Node2D)BRIDGE_UP.Instantiate();
                    bridge_part.Position = new Vector2(start.X, start.Y + bridge_size_width * i);
                    bridges_parent.AddChild(bridge_part);
                }

                //Bridge Start and End Tile
                bridge_part = (Node2D)BRIDGE_UP_END.Instantiate();
                bridge_part.Position = new Vector2(start.X, start.Y + bridge_size_width * -1);
                bridges_parent.AddChild(bridge_part);

                bridge_part = (Node2D)BRIDGE_BOTTOM_END.Instantiate();
                bridge_part.Position = new Vector2(start.X, start.Y + bridge_size_width * (x + 1));
                bridge_part.Scale = new Vector2(-1f, 1f);
                bridges_parent.AddChild(bridge_part);

                break;
        }
    }
}
