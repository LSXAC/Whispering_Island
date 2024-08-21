using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;

public partial class island_menu : CanvasLayer
{
    private PackedScene island_1 = ResourceLoader.Load<PackedScene>("res://Prefabs/Island_1.tscn");
    private PackedScene island_2 = ResourceLoader.Load<PackedScene>("res://Prefabs/Island_2.tscn");

    [Export]
    private Button button1;

    [Export]
    private Button button2;

    public enum ISLANDS
    {
        ISLAND_0,
        ISLAND_1
    };

    public Sign current_sign = null;
    public static island_menu instance;

    public override void _Ready()
    {
        instance = this;
        button1.Pressed += () => SelectIsland(0);
        button2.Pressed += () => SelectIsland(1);
    }

    public void SwitchBuildingMenu()
    {
        if (this.Visible)
            this.Visible = false;
        else
        {
            this.Visible = true;
            // Update Panel with Islands (for : PackedScenes List)
        }
    }

    public void SelectIsland(int id)
    {
        if (current_sign == null)
            return;

        Islands_Manager.INSTANCE.SaveIsland(
            current_sign.dir,
            current_sign.current_ip.unique_island_id,
            id
        );
        CreateIsland(id, current_sign.dir, current_sign.current_ip);

        SwitchBuildingMenu();
        current_sign = null;
    }

    public void CreateIsland(
        int unique_id,
        Island_Properties.DIRECTION dir,
        Island_Properties current_ip
    )
    {
        switch (unique_id)
        {
            case 0:
                current_ip.CreateAnotherIsland(island_1, dir);
                break;
            case 1:
                current_ip.CreateAnotherIsland(island_2, dir);
                break;
        }
    }
}
