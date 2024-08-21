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

    private PackedScene building_typ = ResourceLoader.Load<PackedScene>(
        "res://Menus/building_type.tscn"
    );
    private PackedScene building_1 = ResourceLoader.Load<PackedScene>(
        "res://Placeable/building.tscn"
    );
    private PackedScene building_2 = ResourceLoader.Load<PackedScene>(
        "res://Placeable/Tree_Growther.tscn"
    );

    private PackedScene belt_top = ResourceLoader.Load<PackedScene>(
        "res://Placeable/Belt_Top.tscn"
    );

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitBuildings(building_1);
        InitBuildings(building_2);
        InitBuildings(belt_top);
        instance = this;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Game_Manager.in_building_mode)
            return;

        if (Input.IsActionJustPressed("OpenBuilding"))
            OpenWindow();
    }

    public void OpenWindow()
    {
        this.Visible = true;
    }

    private void InitBuildings(PackedScene scene)
    {
        building_type node = building_typ.Instantiate() as building_type;
        node.InitBuildingTypeUI(scene);
        building_typ_parent.AddChild(node);
    }
}
