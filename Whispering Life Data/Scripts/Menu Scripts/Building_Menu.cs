using Godot;
using Godot.Collections;

public partial class Building_Menu : CanvasLayer
{
    [Export]
    public Building_Placer building_placer;

    [Export]
    private VBoxContainer buildings_parent;

    [Export]
    public Array<PackedScene> buildings = new Array<PackedScene>();
    public static Building_Menu instance;

    private PackedScene buildingMenuChild = ResourceLoader.Load<PackedScene>(
        "res://building_menu_child.tscn"
    );

    public override void _Ready()
    {
        instance = this;
    }

    public void OnVisiblityChange()
    {
        ReloadUIRecipes();
    }

    public override void _Process(double delta)
    {
        if (Game_Manager.building_mode != Game_Manager.BuildingMode.None || Game_Manager.gameover)
            return;

        if (Input.IsActionJustPressed("OpenBuilding"))
            OpenWindow();
    }

    public void ReloadUIRecipes()
    {
        foreach (Control c in buildings_parent.GetChildren())
            c.QueueFree();

        for (int i = 0; i < buildings.Count; i++)
            InitBuildings(buildings[i]);
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

    private void InitBuildings(BuildingType building_type, int id)
    {
        BuildingMenuChild node = buildingMenuChild.Instantiate() as BuildingMenuChild;
        node.InitBuildingMenuChild(building_type, this, id);
        buildings_parent.AddChild(node);
    }
}
