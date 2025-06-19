using Godot;
using Godot.Collections;

public partial class BuildMenu : CanvasLayer
{
    [Export]
    public Building_Placer building_placer;

    [Export]
    public Array<PackedScene> buildings = new Array<PackedScene>();
    public static BuildMenu instance;

    [Export]
    public BuildMenuPageObject production_page,
        decoration_page,
        planting_page,
        research_page;

    public enum CATEGORY
    {
        PRODUCTION,
        DECORATION,
        PLANTING,
        RESEARCH,
        NONE
    }

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
        production_page.Visible = false;
        decoration_page.Visible = false;
        research_page.Visible = false;
        planting_page.Visible = false;
        switch (id)
        {
            case 0:
                production_page.Visible = true;
                break;
            case 1:
                decoration_page.Visible = true;
                break;
            case 2:
                planting_page.Visible = true;
                break;
            case 3:
                research_page.Visible = true;
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

    public void OpenBuildingMenu()
    {
        if (GameMenu.IsWindowActiv())
            return;

        OpenWindow();
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
