using Godot;
using Godot.Collections;

public partial class Building_Menu : CanvasLayer
{
    [Export]
    public Building_Placer building_placer;

    [Export]
    public Array<PackedScene> buildings = new Array<PackedScene>();
    public static Building_Menu instance;

    [Export]
    public BuildingMenuCategory production_panel,
        decoration_panel,
        planting_panel,
        research_panel;

    public override void _Ready()
    {
        instance = this;
    }

    public void OnVisiblityChange()
    {
        OnPanelButton(0);
    }

    public void OnPanelButton(int id)
    {
        production_panel.Visible = false;
        decoration_panel.Visible = false;
        research_panel.Visible = false;
        planting_panel.Visible = false;
        switch (id)
        {
            case 0:
                production_panel.Visible = true;
                break;
            case 1:
                decoration_panel.Visible = true;
                break;
            case 2:
                planting_panel.Visible = true;
                break;
            case 3:
                research_panel.Visible = true;
                break;
        }
    }

    public override void _Process(double delta)
    {
        if (
            Game_Manager.building_mode != Game_Manager.BuildingMode.None
            || Game_Manager.gameover
            || Game_Manager.In_Cutscene
        )
            return;

        if (Input.IsActionJustPressed("OpenBuilding"))
        {
            if (GameMenu.IsWindowActiv())
                return;

            OpenWindow();
        }

        if (Input.IsActionJustPressed("Escape"))
            if (GameMenu.IsThisWindow(this))
            {
                CloseWindow();
            }
    }

    public void OpenWindow()
    {
        GameMenu.SetWindow(this);
    }

    public void CloseWindow()
    {
        GameMenu.CloseLastWindow();
    }

    public void OnRemoveButton()
    {
        this.Visible = false;
        Game_Manager.building_mode = Game_Manager.BuildingMode.Removing;
        player_ui.INSTANCE.SetWindowFrame();
    }
}
