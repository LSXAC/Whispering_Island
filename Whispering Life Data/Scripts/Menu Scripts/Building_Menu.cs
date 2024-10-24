using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;

public partial class Building_Menu : CanvasLayer
{
    [Export]
    public Building_Placer building_placer;

    [Export]
    private GridContainer building_typ_parent;
    public static Building_Menu instance;

    public PackedScene building_typ = ResourceLoader.Load<PackedScene>(
        "res://Menus/building_type.tscn"
    );
    public PackedScene tree_growther = ResourceLoader.Load<PackedScene>(
        "res://Placeable/Tree_Growther.tscn"
    );
    public PackedScene furnace = ResourceLoader.Load<PackedScene>("res://Placeable/Furnace.tscn");
    public PackedScene quarry = ResourceLoader.Load<PackedScene>("res://Placeable/Quarry.tscn");
    public PackedScene chest = ResourceLoader.Load<PackedScene>("res://Placeable/Chest.tscn");

    public PackedScene belt = ResourceLoader.Load<PackedScene>("res://Placeable/Belt.tscn");
    public PackedScene beltItem = ResourceLoader.Load<PackedScene>("res://belt_item.tscn");
    public PackedScene wooden_bed = ResourceLoader.Load<PackedScene>(
        "res://Placeable/WoodenBed.tscn"
    );
    public PackedScene beltTunnel = ResourceLoader.Load<PackedScene>(
        "res://Placeable/BeltTunnel.tscn"
    );

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitBuildings(tree_growther);
        InitBuildings(furnace);
        InitBuildings(quarry);
        InitBuildings(chest);
        InitBuildings(belt);
        InitBuildings(wooden_bed);
        InitBuildings(beltTunnel);
        instance = this;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Game_Manager.building_mode != Game_Manager.BuildingMode.None || Game_Manager.gameover)
            return;

        if (Input.IsActionJustPressed("OpenBuilding"))
            OpenWindow();
    }

    public void OpenWindow()
    {
        this.Visible = true;
    }

    public void OnRemoveButton()
    {
        this.Visible = false;
        Game_Manager.building_mode = Game_Manager.BuildingMode.Removing;
        player_ui.INSTANCE.SetWindowFrame();
    }

    private void InitBuildings(PackedScene scene)
    {
        building_type node = building_typ.Instantiate() as building_type;
        node.InitBuildingTypeUI(scene);
        building_typ_parent.AddChild(node);
    }
}
