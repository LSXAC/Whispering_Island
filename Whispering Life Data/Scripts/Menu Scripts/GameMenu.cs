using System;
using Godot;

public partial class GameMenu : CanvasLayer
{
    [Export]
    public Button load_button;

    [Export]
    public Button inventory_tab_button;

    [Export]
    public Button crafting_tab_button;

    [Export]
    public ColorRect inventory_tab;

    [Export]
    public ColorRect crafting_tab;

    [Export]
    public FurnaceTab furnace_tab;
    public static GameMenu INSTANCE = null;

    public override void _Ready()
    {
        INSTANCE = this;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Escape"))
        {
            if (!Visible)
            {
                this.Visible = true;
                Game_Manager.inside_game_menu = true;
            }
            else
                OnExitButton();
        }
    }

    public void OnVisiblityChange()
    {
        if (SaveState.HasSave())
            load_button.Disabled = false;
        load_button.Disabled = true;
    }

    public void OnExitButton()
    {
        this.Visible = false;
        Game_Manager.inside_game_menu = false;
    }

    public void OnInventoryTabButton()
    {
        crafting_tab.Visible = false;
        inventory_tab.Visible = true;
    }

    public void OnCraftingTabButton()
    {
        crafting_tab.Visible = true;
        inventory_tab.Visible = false;
        crafting_tab.GetNode<CraftingMenu>("CraftingMenuBasic").ReloadUIRecipes();
    }

    public void OnOpenFurnaceTab()
    {
        Visible = true;
        Game_Manager.inside_game_menu = true;
        inventory_tab.Visible = true;
        crafting_tab.Visible = false;
        furnace_tab.Visible = true;
    }

    public void OnCloseFurnaceTab()
    {
        crafting_tab.Visible = false;
        furnace_tab.Visible = false;
        Game_Manager.inside_game_menu = false;
        Visible = false;
    }

    public void OnSaveButton()
    {
        Game_Manager.INSTANCE.SaveGame();
    }

    public void OnLoadButton()
    {
        Game_Manager.INSTANCE.LoadGame();
    }
}
