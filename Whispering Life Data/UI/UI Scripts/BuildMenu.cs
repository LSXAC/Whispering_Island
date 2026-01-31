using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class BuildMenu : CanvasLayer
{
    [Export]
    public BuildingPlacer building_placer;

    public static BuildMenu instance;

    [Export]
    public HBoxContainer[] hbox_category;

    [Export]
    public BuildMenuPageObject production_page,
        decoration_page,
        planting_page,
        research_page,
        transport_page,
        machine_page;

    public enum CATEGORY
    {
        PRODUCTION,
        DECORATION,
        PLANTING,
        RESEARCH,
        ADMIN,
        TRANSPORT,
        MACHINES,
        NONE
    }

    public override void _Ready()
    {
        instance = this;
        if (this.Visible)
            this.Visible = false;
    }

    public void OnVisiblityChange()
    {
        OnPanelButton(4);
    }

    public void OnPanelButton(int id)
    {
        production_page.Visible = false;
        decoration_page.Visible = false;
        research_page.Visible = false;
        planting_page.Visible = false;
        transport_page.Visible = false;
        machine_page.Visible = false;
        //hbox_category[0].Visible = false;
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
            case 4:
                transport_page.Visible = true;
                break;
            case 5:
                machine_page.Visible = true;
                break;
        }
    }

    public void OnCategoryButton(int id)
    {
        Debug.Print("ID: " + id);
        //hbox_category[id].Visible = true;
        if (id == 0)
            OnPanelButton(4);
    }

    public override void _Process(double delta)
    {
        if (
            GameManager.building_mode != GameManager.BuildingMode.None
            || GameManager.gameover
            || GameManager.In_Cutscene
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
                CloseWindow();
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
        GameManager.building_mode = GameManager.BuildingMode.Removing;
        PlayerUI.instance.SetWindowFrame();
    }
}
