using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;

public partial class island_menu : ColorRect
{
    private PackedScene island_1 = ResourceLoader.Load<PackedScene>("res://Prefabs/Island_1.tscn");
    private PackedScene island_2 = ResourceLoader.Load<PackedScene>("res://Prefabs/Island_2.tscn");
    private PackedScene island_3 = ResourceLoader.Load<PackedScene>("res://Prefabs/Island_3.tscn");
    private PackedScene island_4 = ResourceLoader.Load<PackedScene>(
        "res://Prefabs/Island_4_Dessert.tscn"
    );

    [Export]
    private Button button1;

    [Export]
    private Button button2;

    [Export]
    private Button button3;

    [Export]
    private Button button4;

    public enum ISLANDS
    {
        ISLAND_0,
        ISLAND_1,
        ISLAND_2,
        ISLAND_3
    };

    public Sign current_sign = null;
    public static island_menu instance;

    public override void _Ready()
    {
        instance = this;
        button1.Pressed += () => SelectIsland(0);
        button2.Pressed += () => SelectIsland(1);
        button3.Pressed += () => SelectIsland(2);
        button4.Pressed += () => SelectIsland(3);
    }

    public void SelectIsland(int id)
    {
        if (current_sign == null)
            return;

        CreateIsland(id, current_sign.dir, current_sign.current_ip);

        GameMenu.INSTANCE.OnCloseIslandTab();
        current_sign = null;
    }

    public void CreateIsland(
        int unique_id,
        Island_Properties.DIRECTION dir,
        Island_Properties current_ip,
        bool is_loading = false
    )
    {
        Debug.Print("Get Dir.");
        if (dir == Island_Properties.DIRECTION.UP)
            if (Game_Manager.IsIslandOnMatrix(current_ip.matrix_x, current_ip.matrix_y - 1))
                return;
        if (dir == Island_Properties.DIRECTION.DOWN)
            if (Game_Manager.IsIslandOnMatrix(current_ip.matrix_x, current_ip.matrix_y + 1))
                return;
        if (dir == Island_Properties.DIRECTION.LEFT)
            if (Game_Manager.IsIslandOnMatrix(current_ip.matrix_x - 1, current_ip.matrix_y))
                return;
        if (dir == Island_Properties.DIRECTION.RIGHT)
            if (Game_Manager.IsIslandOnMatrix(current_ip.matrix_x + 1, current_ip.matrix_y))
                return;

        Debug.Print("Found Dir.");
        switch (unique_id)
        {
            case 0:
                Debug.Print("Island 0.");
                current_ip.CreateAnotherIsland(island_1, dir, is_loading);
                break;
            case 1:
                Debug.Print("Island 1.");
                current_ip.CreateAnotherIsland(island_2, dir, is_loading);
                break;
            case 2:
                Debug.Print("Island 2.");
                current_ip.CreateAnotherIsland(island_3, dir, is_loading);
                break;
            case 3:
                Debug.Print("Island 3.");
                current_ip.CreateAnotherIsland(island_4, dir, is_loading);
                break;
            default:
                Debug.Print("No matching Island ID found");
                break;
        }
    }
}
