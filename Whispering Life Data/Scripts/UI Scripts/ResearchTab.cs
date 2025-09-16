using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class ResearchTab : ColorRect
{
    public static Dictionary<Inventory.ITEM_ID, ResearchSave> research_saves =
        new Dictionary<Inventory.ITEM_ID, ResearchSave>();

    [Export]
    public TextureRect view_icon;

    [Export]
    public ColorRect sub_all_research_panel;

    [Export]
    public ColorRect no_items_discovered_panel;

    [Export]
    public ColorRect full_researched_panel;

    [Export]
    public Array<Control> sub_research_panel_types;

    [Export]
    public Label research_current_title,
        research_current_time_left;

    [Export]
    public ProgressBar research_current_progress;

    [Export]
    public ItemRowManager item_row_manager;

    [Export]
    public Label research_title,
        research_description;

    [Export]
    public Label research_subresearches_left;

    [Export]
    public ColorRect research_activ_rect;

    [Export]
    public GridContainer select_container;

    [Export]
    public Timer timer;

    [Export]
    public Color select_color,
        unselect_color,
        complete_color;
    public PackedScene research_level_tab = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/Research_Level_Tab.tscn"
    );
    public PackedScene classic_button = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/research_select_tab_button.tscn"
    );

    public Array<ResearchSave> research_states = new Array<ResearchSave>();
    public static ItemSave research_slot_item = null;

    public static ResearchTab instance = null;

    [Export]
    public int Research_Points = 0;

    [Export]
    public int sub_current_progress_time = 0;

    [Export]
    public Button start_research_button;

    [Export]
    public Texture2D star_full_texture,
        star_empty_texture;

    public static ItemInfo current_selected_research_info = null;
    public static ItemResearch current_research = null;
    public static int current_research_level = -1;

    public Dictionary<Inventory.ITEM_ID, ItemInfo> researchable_items =
        new Dictionary<Inventory.ITEM_ID, ItemInfo>();
    bool in_upgrade_knowledge = false;

    public static int selected_sub_id = -1;
    public static Inventory.ITEM_ID current_selected_item_id = Inventory.ITEM_ID.NULL;
    public static bool in_research = false;
    public static int current_research_prog = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Research_Points = 0;
        instance = this;
        //ClearText();
        start_research_button.Pressed += () => OnResearchButton();
    }

    public void UpdateSelectTab()
    {
        researchable_items.Clear();
        bool has_entry = false;
        foreach (Node node in select_container.GetChildren())
            node.QueueFree();

        Array<ItemInfo> left_infos = new Array<ItemInfo>();
        foreach (var kvp in Inventory.ITEM_TYPES)
        {
            ItemInfo info = kvp.Value;
            if (info.GetAttributeOrNull<ResearchableAttribute>() == null)
                continue;

            if (Database.researchs.ContainsKey(info.id))
            {
                if (DiscoverManager.discovered_items.ContainsKey(info.id))
                {
                    if (current_selected_item_id == Inventory.ITEM_ID.NULL)
                        current_selected_item_id = info.id;

                    if (research_saves.ContainsKey(info.id))
                        current_research_level = research_saves[info.id].research_level;

                    Button btn = classic_button.Instantiate() as Button;
                    for (int i = 0; i < Database.researchs[info.id].item_research_levels.Count; i++)
                    {
                        btn.GetChild(0).GetChild<TextureRect>(i).Visible = true;
                        btn.GetChild(0).GetChild<TextureRect>(i).Texture =
                            i < current_research_level ? star_full_texture : star_empty_texture;
                    }

                    researchable_items[info.id] = info;
                    select_container.AddChild(btn);
                    btn.Icon = info.texture;
                    btn.Disabled = false;
                    btn.Pressed += () => OnSelectItem(info.id);
                    has_entry = true;
                }
                else
                    left_infos.Add(info);
            }
        }
        foreach (ItemInfo info in left_infos)
        {
            researchable_items[info.id] = info;
            Button btn = classic_button.Instantiate() as Button;
            btn.Text = "?";
            btn.Icon = null;
            btn.Disabled = true;
            select_container.AddChild(btn);
        }
        if (has_entry)
        {
            no_items_discovered_panel.Visible = false;
            OnSelectItem(current_selected_item_id);
        }
        else
            no_items_discovered_panel.Visible = true;
    }

    public void OnVisiblityChanged()
    {
        UpdateSelectTab();
    }

    public void OnSelectItem(Inventory.ITEM_ID id)
    {
        foreach (Control ctrl in sub_research_panel_types)
            ctrl.Visible = false;

        sub_all_research_panel.Visible = false;
        full_researched_panel.Visible = false;

        ItemInfo info = researchable_items[id];
        current_selected_research_info = info;
        current_research = Database.researchs[id];
        current_selected_item_id = id;

        view_icon.Texture = info.texture;

        current_research_level = 0;
        if (research_saves.ContainsKey(id))
            current_research_level = research_saves[id].research_level;

        if (current_research_level >= Database.researchs[id].item_research_levels.Count)
        {
            start_research_button.Text = "//////";
            research_subresearches_left.Text = "///////////";
            research_title.Text = "////// ";
            research_description.Text = "///////////";
            full_researched_panel.Visible = true;
            return;
        }

        int type = (int)id % sub_research_panel_types.Count;
        sub_research_panel_types[type].Visible = true;

        int subs_left = 5;

        for (
            int x = 0;
            x < current_research.item_research_levels[current_research_level].sub_levels.Count;
            x++
        )
        {
            ItemSubResearchLevel sub = current_research
                .item_research_levels[current_research_level]
                .sub_levels[x];
            SubResearchPanel panel = sub_research_panel_types[type].GetChild<SubResearchPanel>(x);
            panel.InitPanel(sub, x);
            panel.SetColor(unselect_color);

            if (research_saves.ContainsKey(id))
                if (research_saves[id].sub_level_progress.ContainsKey(sub.category))
                {
                    subs_left -= 1;
                    panel.DisableButton();
                    panel.SetColor(complete_color);
                }
        }

        if (in_research)
        {
            if (selected_sub_id != -1)
            {
                OnSelectSubResearch(selected_sub_id);
                OnResearchButton();
            }
            else
                OnUpgradeButton();
            UpdateResearchProgress();
            return;
        }

        foreach (Control c in item_row_manager.GetChildren())
            c.QueueFree();

        research_title.Text = "Title MAIN: " + id;
        research_description.Text = "Description MAIN: " + id;
        start_research_button.Text = TranslationServer.Translate("RESEARCH_BUTTON");
        research_subresearches_left.Text =
            TranslationServer.Translate("RESEARCH_UPGRADE_BUTTON_DESC")
            + " "
            + (5 - subs_left)
            + "/5 "
            + TranslationServer.Translate("RESEARCH_UPGRADE_BUTTON_DESC_2");

        if (!in_research)
            if (subs_left <= 0)
            {
                sub_all_research_panel.Visible = true;
                start_research_button.Text = TranslationServer.Translate("RESEARCH_UPGRADE_BUTTON");
                research_subresearches_left.Text = TranslationServer.Translate(
                    "RESEARCH_UPGRADE_BUTTON_NOW_UPGRADE"
                );
                start_research_button.Pressed += () => OnUpgradeButton();
                selected_sub_id = -1;
            }
    }

    public void DeselectAllSubPanales()
    {
        int type = (int)current_selected_item_id % sub_research_panel_types.Count;

        for (
            int x = 0;
            x < current_research.item_research_levels[current_research_level].sub_levels.Count;
            x++
        )
        {
            ItemSubResearchLevel sub = current_research
                .item_research_levels[current_research_level]
                .sub_levels[x];
            SubResearchPanel panel = sub_research_panel_types[type].GetChild<SubResearchPanel>(x);
            if (research_saves.ContainsKey(current_selected_item_id))
                if (
                    research_saves[current_selected_item_id]
                        .sub_level_progress.ContainsKey(sub.category)
                )
                    continue;

            panel.SetColor(unselect_color);
        }
    }

    public void OnSelectSubResearch(int id)
    {
        selected_sub_id = id;
        research_title.Text = "SUB Title: " + id;
        research_description.Text = "SUB Description: " + id;
        item_row_manager.SetResourcesOnUI(
            new Array<Item> { new Item(current_selected_research_info, 5) }
        );
    }

    public void OnResearchButton()
    {
        if (selected_sub_id == -1)
            return;

        in_research = true;
        research_activ_rect.Visible = true;
        research_current_title.Text = current_research
            .item_research_levels[current_research_level]
            .sub_levels[selected_sub_id]
            .category.ToString();

        if (current_research_prog != 0)
            research_current_progress.Value = current_research_prog;
        else
            research_current_progress.Value = 0;

        research_current_progress.MaxValue = current_research
            .item_research_levels[current_research_level]
            .sub_levels[selected_sub_id]
            .time_needed;
        timer.Start();
    }

    public void OnUpgradeButton()
    {
        in_research = true;
        sub_all_research_panel.Visible = false;
        in_upgrade_knowledge = true;
        research_activ_rect.Visible = true;
        research_current_title.Text = TranslationServer.Translate(
            "RESEARCH_CURRENT_UPGRADE_LEVELING"
        );

        if (current_research_prog != 0)
            research_current_progress.Value = current_research_prog;
        else
            research_current_progress.Value = 0;

        research_current_progress.MaxValue = current_research
            .item_research_levels[current_research_level]
            .time_needed;
        timer.Start();
    }

    public void OnTimerTimeout()
    {
        if (research_current_progress.Value < research_current_progress.MaxValue)
            UpdateResearchProgress();
        else
        {
            current_research_prog = 0;
            in_research = false;
            timer.Stop();
            OnResearchFinished();
        }
    }

    public void UpdateResearchProgress()
    {
        current_research_prog += 1;
        research_current_progress.Value = current_research_prog;
        research_current_time_left.Text =
            "Time: "
            + research_current_progress.Value.ToString()
            + " / "
            + research_current_progress.MaxValue.ToString()
            + " s";
    }

    public void OnResearchFinished()
    {
        Inventory.ITEM_ID id = current_selected_research_info.id;
        /*else if (
            research_saves[current_selected_research_info.id].research_level
            >= Database.researchs[current_selected_research_info.id].item_research_levels.Count
        )
        {
            full_researched_panel.Visible = true;
        }*/
        if (in_upgrade_knowledge)
        {
            research_saves[id].research_level += 1;
            research_saves[id].sub_level_progress.Clear();
            start_research_button.Pressed += () => OnResearchButton();
            in_upgrade_knowledge = false;
        }
        else
        {
            if (!research_saves.ContainsKey(id))
                research_saves[id] = new ResearchSave();

            research_saves[id]
                .sub_level_progress.Add((ItemSubResearchLevel.CATEGORY)selected_sub_id, true);
        }

        research_activ_rect.Visible = false;
        Research_Points += 1;

        UpdateSelectTab();
    }
}
