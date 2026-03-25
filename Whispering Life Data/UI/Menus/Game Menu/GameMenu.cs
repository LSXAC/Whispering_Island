using System;
using System.Diagnostics;
using Godot;

public partial class GameMenu : CanvasLayer
{
    [Export]
    public Button load_button;

    [Export]
    public DestroyMenu destroy_menu;

    [Export]
    public Button inventory_tab_button;

    [Export]
    public Button crafting_tab_button;

    [Export]
    public ColorRect inventory_tab;

    [Export]
    public ColorRect crafting_tab;

    [Export]
    public ProcessingTab processing_tab;

    [Export]
    public ResearchTab research_tab;

    [Export]
    public NerveTransducerTab nerve_transducer_tab;

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

    [Export]
    public ColorRect quest_tab;

    [Export]
    public ColorRect rail_station_tab;

    [Export]
    public ColorRect minecart_tab;

    public static GameMenu instance = null;

    [Export]
    public HBoxContainer header_container;

    [Export]
    public LabelSettings title_selected,
        title_normal;

    [Export]
    public CraftingCategoryPanel crafting_category_panel;

    public static QuestMenu questMenu;

    public override void _Ready()
    {
        questMenu = (QuestMenu)quest_tab;
        instance = this;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (GameManager.gameover || CutsceneManager.In_Cutscene)
            return;

        if (Input.IsActionJustPressed("Inventory"))
        {
            OpenGameMenu();
        }
        if (Input.IsActionJustPressed("Escape"))
        {
            if (IsThisWindow(this))
            {
                PlayerInventoryUI.instance?.MarkSlotsWithAttributeTypes(null);
                CloseLastWindow();
                OnExitButton();
                return;
            }
        }
    }

    public void OpenGameMenu()
    {
        if (IsWindowActiv())
            return;

        if (PlayerInventoryUI.instance != null)
            PlayerInventoryUI.instance.MarkSlotsWithAttributeTypes(null);
        ChangeSelectedTabColor(Tabs.Inventory);
        SetWindow(this);
    }

    public async void OnBackToMainMenu()
    {
        TransitionManager.StartTransition();
        await TransitionManager.IsInTransitionLoop();

        MainMenu.instance.parent.Visible = true;
        MainMenu.instance.OnBackToMainMenu();
        GameManager.instance.QueueFree();

        TransitionManager.instance.StopTransition();
    }

    public static void CloseLastWindow()
    {
        if (GameManager.current_activ_canvaslayer != null)
        {
            Debug.Print(GameManager.current_activ_canvaslayer.Name);
            GameManager.current_activ_canvaslayer.Visible = false;
            GameManager.current_activ_canvaslayer = null;
            Debug.Print("Last Window Closed");
        }
    }

    public static bool IsThisWindow(CanvasLayer layer)
    {
        if (GameManager.current_activ_canvaslayer != null)
            if (GameManager.current_activ_canvaslayer == layer)
                return true;

        return false;
    }

    public static bool IsWindowActiv()
    {
        if (GameManager.current_activ_canvaslayer == null)
            return false;
        return true;
    }

    public static void SetWindow(CanvasLayer node)
    {
        Debug.Print(node.Name);
        GameManager.current_activ_canvaslayer = node;
        GameManager.current_activ_canvaslayer.Visible = true;
    }

    enum Tabs
    {
        Inventory,
        Crafting,
        LoadSave,
        Stats,
        SkillTree,
        Admin,
        Guide,
        Settings,
        X
    }

    private void ChangeSelectedTabColor(Tabs tab)
    {
        foreach (Button btn in header_container.GetChildren())
        {
            if (btn.HasNode("HBoxContainer"))
                btn.GetChild(0).GetNode<Label>("Label").LabelSettings = title_normal;
        }
        ((Button)header_container.GetChild((int)tab))
            .GetChild(0)
            .GetNode<Label>("Label")
            .LabelSettings = title_selected;
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
        //ChangeSelectedTabColor(Tabs.Settings);
        settings_tab.Visible = true;
    }

    public void OnCraftingTabButton()
    {
        SetWindow(this);
        ChangeSelectedTabColor(Tabs.Crafting);
        CloseAllTabs();
        crafting_tab.Visible = true;
        Debug.Print("OnTab0");
        OnCraftingCategoryButton(0);
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuBasic").ReloadUIRecipes();
        crafting_category_panel.SetButton((CraftingCategoryPanel.CATEGORIES)0);
    }

    public void OnCraftingCategoryButton(int id)
    {
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuBasic").Visible = false;
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuTools").Visible = false;
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuToolparts").Visible = false;
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuArmor").Visible = false;
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuAgriculture").Visible = false;
        crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuMachineParts").Visible = false;
        crafting_category_panel.SetButton((CraftingCategoryPanel.CATEGORIES)id);

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
            case 5:
                crafting_tab.GetChild(0).GetNode<CraftingMenu>("CraftingMenuMachineParts").Visible =
                    true;
                crafting_tab
                    .GetChild(0)
                    .GetNode<CraftingMenu>("CraftingMenuMachineParts")
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

    public void OnOpenProcessingTab()
    {
        SetWindow(this);
        CloseAllTabs();
        inventory_tab.Visible = true;
        processing_tab.Visible = true;
        PlayerInventoryUI.instance.MarkSlotsWithAttributeTypes(
            new Type[] { typeof(BurnableAttribute), typeof(SmeltableAttribute) }
        );
    }

    public void OnOpenNerveTranducerTab()
    {
        SetWindow(this);
        CloseAllTabs();
        inventory_tab.Visible = true;
        nerve_transducer_tab.Visible = true;
    }

    public void OnOpenChestTab()
    {
        SetWindow(this);
        CloseAllTabs();
        inventory_tab.Visible = true;
        chest_tab.Visible = true;
    }

    public void OnOpenRailStationTab()
    {
        SetWindow(this);
        CloseAllTabs();
        inventory_tab.Visible = true;
        rail_station_tab.Visible = true;
    }

    public void OnOpenMinecartTab()
    {
        SetWindow(this);
        CloseAllTabs();
        inventory_tab.Visible = true;
        minecart_tab.Visible = true;
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
        processing_tab.Visible = false;
        nerve_transducer_tab.Visible = false;
        admin_tab.Visible = false;
        skilltree_tab.Visible = false;
        settings_tab.Visible = false;
        saveload_tab.Visible = false;
        island_tab.Visible = false;
        chest_tab.Visible = false;
        help_tab.Visible = false;
        rail_station_tab.Visible = false;
        minecart_tab.Visible = false;
        destroy_menu.Visible = false;
        quest_tab.Visible = false;
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
        research_tab.Visible = true;
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
        ChestInventory.current_chest = null;
        CloseLastWindow();
    }

    public void OnCloseDestroyTab()
    {
        CloseAllTabs();
        inventory_tab.Visible = true;
        CloseLastWindow();
    }

    public void OnOpenIslandTab()
    {
        SetWindow(this);
        CloseAllTabs();
        island_tab.Visible = true;
    }

    public void OnOpenQuestTab()
    {
        SetWindow(this);
        CloseAllTabs();
        quest_tab.Visible = true;
        inventory_tab.Visible = true;
    }

    public void OnOpenDestroyTab()
    {
        SetWindow(this);
        CloseAllTabs();
        destroy_menu.Visible = true;
    }

    public void OnCloseIslandTab()
    {
        CloseAllTabs();
        inventory_tab.Visible = true;
        CloseLastWindow();
    }

    public void OnSaveButton()
    {
        GameManager.instance.SaveGame();
    }

    public void OnLoadButton()
    {
        MainMenu.instance.OnLoadGameButtoN();
    }
}
