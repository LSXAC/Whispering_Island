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

    [Export]
    public ColorRect chest_tab;

    [Export]
    public ColorRect settings_tab;

    [Export]
    public ColorRect saveload_tab;

    public static GameMenu INSTANCE = null;

    [Export]
    public HBoxContainer header_container;

    public Color font_color_selected = new Color(0.969f, 0.78f, 0.4f);
    public Color font_color_standard = new Color(1f, 1f, 1);

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
                ChangeSelectedTabColor(Tabs.Inventory);
                this.Visible = true;
                Game_Manager.inside_game_menu = true;
            }
            else
            {
                ChangeSelectedTabColor(Tabs.X);
                OnExitButton();
            }
        }
    }

    enum Tabs
    {
        Inventory,
        Crafting,
        Stats,
        LoadSave,
        Settings,
        X
    }

    private void ChangeSelectedTabColor(Tabs tab)
    {
        foreach (Button btn in header_container.GetChildren())
        {
            btn.RemoveThemeColorOverride("font_color");
            btn.RemoveThemeColorOverride("font_hover_color");
        }
        ((Button)header_container.GetChild((int)tab)).AddThemeColorOverride(
            "font_color",
            font_color_selected
        );
        ((Button)header_container.GetChild((int)tab)).AddThemeColorOverride(
            "font_hover_color",
            font_color_selected
        );
    }

    public void OnVisiblityChange()
    {
        if (SaveState.HasSave())
            load_button.Disabled = false;
        load_button.Disabled = true;
    }

    public void OnExitButton()
    {
        ChangeSelectedTabColor(Tabs.X);
        OnCloseFurnaceTab();
        this.Visible = false;
        Game_Manager.inside_game_menu = false;
    }

    public void OnInventoryTabButton()
    {
        ChangeSelectedTabColor(Tabs.Inventory);
        crafting_tab.Visible = false;
        inventory_tab.Visible = true;
        settings_tab.Visible = false;
        saveload_tab.Visible = false;
    }

    public void OnSettingsTabButton()
    {
        ChangeSelectedTabColor(Tabs.Settings);
        crafting_tab.Visible = false;
        inventory_tab.Visible = false;
        settings_tab.Visible = true;
        saveload_tab.Visible = false;
    }

    public void OnCraftingTabButton()
    {
        ChangeSelectedTabColor(Tabs.Crafting);
        crafting_tab.Visible = true;
        inventory_tab.Visible = false;
        settings_tab.Visible = false;
        saveload_tab.Visible = false;
        crafting_tab.GetNode<CraftingMenu>("CraftingMenuBasic").ReloadUIRecipes();
    }

    public void OnSaveLoadTabButton()
    {
        ChangeSelectedTabColor(Tabs.LoadSave);
        crafting_tab.Visible = false;
        inventory_tab.Visible = false;
        settings_tab.Visible = false;
        saveload_tab.Visible = true;
        crafting_tab.GetNode<CraftingMenu>("CraftingMenuBasic").ReloadUIRecipes();
    }

    public void OnOpenFurnaceTab()
    {
        ChangeSelectedTabColor(Tabs.Inventory);
        Visible = true;
        Game_Manager.inside_game_menu = true;
        inventory_tab.Visible = true;
        crafting_tab.Visible = false;
        settings_tab.Visible = false;
        furnace_tab.Visible = true;
        chest_tab.Visible = false;
        saveload_tab.Visible = false;
    }

    public void OnOpenChestTab()
    {
        ChangeSelectedTabColor(Tabs.Inventory);
        Visible = true;
        Game_Manager.inside_game_menu = true;
        inventory_tab.Visible = true;
        crafting_tab.Visible = false;
        settings_tab.Visible = false;
        furnace_tab.Visible = false;
        chest_tab.Visible = true;
        saveload_tab.Visible = false;
    }

    public void OnCloseFurnaceTab()
    {
        ChangeSelectedTabColor(Tabs.X);
        inventory_tab.Visible = true;
        crafting_tab.Visible = false;
        furnace_tab.Visible = false;
        settings_tab.Visible = false;
        saveload_tab.Visible = false;
        Game_Manager.inside_game_menu = false;
        Visible = false;
    }

    public void OnCloseChestTab()
    {
        ChangeSelectedTabColor(Tabs.X);
        inventory_tab.Visible = true;
        crafting_tab.Visible = false;
        furnace_tab.Visible = false;
        settings_tab.Visible = false;
        chest_tab.Visible = false;
        saveload_tab.Visible = false;
        ChestInventory.INSTANCE.current_chest = null;
        Game_Manager.inside_game_menu = false;
        Visible = false;
    }

    public void OnSaveButton()
    {
        Game_Manager.INSTANCE.SaveGame();
    }

    public void OnLoadButton()
    {
        OnExitButton();
        GetTree().ReloadCurrentScene();
    }
}
