using System;
using System.Diagnostics;
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
    public ResearchTab research_tab;

    [Export]
    public ColorRect chest_tab;

    [Export]
    public ColorRect settings_tab;

    [Export]
    public ColorRect saveload_tab;

    [Export]
    public ColorRect admin_tab;

    [Export]
    public ColorRect island_tab;

    [Export]
    public ColorRect skilltree_tab;

    [Export]
    public ColorRect help_tab;

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
    public override void _PhysicsProcess(double delta)
    {
        if (Game_Manager.gameover || Game_Manager.In_Cutscene)
            return;

        if (Input.IsActionJustPressed("Escape"))
        {
            OpenGameMenu();
        }
    }

    public void OpenGameMenu()
    {
        if (IsThisWindow(this))
        {
            Inventory.INSTANCE.MarkSlotsWithType(null);
            CloseLastWindow();
            OnExitButton();
            return;
        }

        if (IsWindowActiv())
            return;

        Inventory.INSTANCE.MarkSlotsWithType(null);
        ChangeSelectedTabColor(Tabs.Inventory);
        SetWindow(this);
    }

    public async void OnBackToMainMenu()
    {
        TransitionManager.StartTransition();
        await TransitionManager.IsInTransitionLoop();

        MainMenu.INSTANCE.parent.Visible = true;
        Game_Manager.INSTANCE.QueueFree();

        TransitionManager.INSTANCE.StopTransition();
    }

    public static void CloseLastWindow()
    {
        if (Game_Manager.current_activ_canvaslayer != null)
        {
            Game_Manager.current_activ_canvaslayer.Visible = false;
            Game_Manager.current_activ_canvaslayer = null;
            Debug.Print("Last Window Closed");
        }
    }

    public static bool IsThisWindow(CanvasLayer layer)
    {
        if (Game_Manager.current_activ_canvaslayer != null)
            if (Game_Manager.current_activ_canvaslayer == layer)
                return true;

        return false;
    }

    public static bool IsWindowActiv()
    {
        if (Game_Manager.current_activ_canvaslayer == null)
            return false;
        return true;
    }

    public static void SetWindow(CanvasLayer node)
    {
        Debug.Print(node.Name);
        Game_Manager.current_activ_canvaslayer = node;
        Game_Manager.current_activ_canvaslayer.Visible = true;
    }

    enum Tabs
    {
        Inventory,
        Crafting,
        Stats,
        SkillTree,
        LoadSave,
        Settings,
        Guide,
        Admin,
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
        CloseAllTabs();
        inventory_tab.Visible = true;
        CloseLastWindow();
    }

    public void OnInventoryTabButton()
    {
        SetWindow(this);
        ChangeSelectedTabColor(Tabs.Inventory);
        CloseAllTabs();
        inventory_tab.Visible = true;
    }

    public void OnHelpTabButton()
    {
        SetWindow(this);
        ChangeSelectedTabColor(Tabs.Guide);
        CloseAllTabs();
        help_tab.Visible = true;
    }

    public void OnSettingsTabButton()
    {
        SetWindow(this);
        CloseAllTabs();
        ChangeSelectedTabColor(Tabs.Settings);
        settings_tab.Visible = true;
    }

    public void OnCraftingTabButton()
    {
        SetWindow(this);
        ChangeSelectedTabColor(Tabs.Crafting);
        CloseAllTabs();
        crafting_tab.Visible = true;
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuBasic").ReloadUIRecipes();
    }

    public void OnCraftingCategoryButton(int id)
    {
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuBasic").Visible = false;
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuTools").Visible = false;
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuToolparts").Visible = false;
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuArmor").Visible = false;
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuAgriculture").Visible = false;

        switch (id)
        {
            case 0:
                crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuBasic").Visible = true;
                crafting_tab
                    .GetChild(0)
                    .GetNode<CraftingMenu>("CraftingMenuBasic")
                    .ReloadUIRecipes();
                break;
            case 1:
                crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuTools").Visible = true;
                crafting_tab
                    .GetChild(0)
                    .GetNode<CraftingMenu>("CraftingMenuTools")
                    .ReloadUIRecipes();
                break;
            case 2:
                crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuToolparts").Visible =
                    true;
                crafting_tab
                    .GetChild(0)
                    .GetNode<CraftingMenu>("CraftingMenuToolparts")
                    .ReloadUIRecipes();
                break;
            case 3:
                crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuArmor").Visible = true;
                crafting_tab
                    .GetChild(0)
                    .GetNode<CraftingMenu>("CraftingMenuArmor")
                    .ReloadUIRecipes();
                break;
            case 4:
                crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuAgriculture").Visible =
                    true;
                crafting_tab
                    .GetChild(0)
                    .GetNode<CraftingMenu>("CraftingMenuAgriculture")
                    .ReloadUIRecipes();
                break;
        }
    }

    public void OnSaveLoadTabButton()
    {
        SetWindow(this);
        ChangeSelectedTabColor(Tabs.LoadSave);
        CloseAllTabs();
        saveload_tab.Visible = true;
    }

    public void OnOpenFurnaceTab()
    {
        SetWindow(this);
        CloseAllTabs();
        inventory_tab.Visible = true;
        furnace_tab.Visible = true;
        Inventory.INSTANCE.MarkSlotsWithType(
            new ItemInfo.Type[] { ItemInfo.Type.BURNABLE, ItemInfo.Type.SMELTABLE }
        );
    }

    public void OnOpenChestTab()
    {
        SetWindow(this);
        CloseAllTabs();
        inventory_tab.Visible = true;
        chest_tab.Visible = true;
    }

    public void OnOpenAdminTab()
    {
        ChangeSelectedTabColor(Tabs.Admin);
        SetWindow(this);
        CloseAllTabs();
        inventory_tab.Visible = true;
        admin_tab.Visible = true;
    }

    public void OnCloseFurnaceTab()
    {
        CloseAllTabs();
        inventory_tab.Visible = true;
        CloseLastWindow();
    }

    private void CloseAllTabs()
    {
        inventory_tab.Visible = false;
        research_tab.Visible = false;
        crafting_tab.Visible = false;
        furnace_tab.Visible = false;
        admin_tab.Visible = false;
        skilltree_tab.Visible = false;
        settings_tab.Visible = false;
        saveload_tab.Visible = false;
        island_tab.Visible = false;
        chest_tab.Visible = false;
        help_tab.Visible = false;
    }

    public void OnCloseResearchTab()
    {
        CloseAllTabs();
        inventory_tab.Visible = true;
        CloseLastWindow();
    }

    public void OnOpenResearchTab()
    {
        SetWindow(this);
        CloseAllTabs();
        inventory_tab.Visible = true;
        research_tab.Visible = true;
        Inventory.INSTANCE.MarkSlotsWithType(new ItemInfo.Type[] { ItemInfo.Type.RESEARCHABLE });
    }

    public void OnCloseSkilltreeTab()
    {
        CloseAllTabs();
        inventory_tab.Visible = true;
        CloseLastWindow();
    }

    public void OnOpenSkilltreeTab()
    {
        SetWindow(this);
        CloseAllTabs();
        ChangeSelectedTabColor(Tabs.SkillTree);
        skilltree_tab.Visible = true;
    }

    public void OnCloseChestTab()
    {
        CloseAllTabs();
        inventory_tab.Visible = true;
        ChestInventory.INSTANCE.current_chest = null;
        CloseLastWindow();
    }

    public void OnOpenIslandTab()
    {
        SetWindow(this);
        CloseAllTabs();
        island_tab.Visible = true;
    }

    public void OnCloseIslandTab()
    {
        CloseAllTabs();
        inventory_tab.Visible = true;
        CloseLastWindow();
    }

    public void OnSaveButton()
    {
        Game_Manager.INSTANCE.SaveGame();
    }

    public void OnLoadButton()
    {
        MainMenu.INSTANCE.OnLoadGameButtoN();
    }
}
