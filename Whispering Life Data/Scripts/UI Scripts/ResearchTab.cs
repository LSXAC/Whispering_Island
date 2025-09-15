using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class ResearchTab : ColorRect
{
    public static Dictionary<Inventory.ITEM_ID, ResearchSave> research_saves =
        new Dictionary<Inventory.ITEM_ID, ResearchSave>();

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
    public PackedScene research_level_tab = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/Research_Level_Tab.tscn"
    );
    public PackedScene classic_button = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/classic_button.tscn"
    );

    public Array<ResearchSave> research_states = new Array<ResearchSave>();
    public static ItemSave research_slot_item = null;

    public static ResearchTab instance = null;

    [Export]
    public int Research_Points = 0;

    [Export]
    public int sub_current_progress_time = 0;

    public static ItemInfo current_selected_research_info = null;
    public static ItemResearch current_research = null;
    public static int selected_id = -1;
    public static int current_research_level = -1;

    public Array<ItemInfo> researchable_items = new Array<ItemInfo>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Research_Points = 0;
        instance = this;
        //ClearText();
    }

    public void UpdateSelectTab()
    {
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
                Button btn = classic_button.Instantiate() as Button;
                researchable_items.Add(info);
                select_container.AddChild(btn);
                btn.Icon = info.texture;
                btn.Disabled = false;

                //Disable, when max level reached
                if (research_saves.ContainsKey(info.id))
                    if (
                        research_saves[info.id].research_level
                        >= Database.researchs[info.id].item_research_levels.Count
                    )
                        btn.Disabled = true;
            }
        }
        foreach (ItemInfo info in left_infos)
        {
            researchable_items.Add(info);
            left_infos.Add(info);
            Button btn = classic_button.Instantiate() as Button;
            btn.Text = "?";
            btn.Icon = null;
            btn.Disabled = true;
            select_container.AddChild(btn);
        }

        foreach (Button btn in select_container.GetChildren())
        {
            int id = btn.GetIndex();
            btn.Pressed += () => OnSelectItem(id);
        }

        OnSelectItem(0);
    }

    public void OnVisiblityChanged()
    {
        UpdateSelectTab();
    }

    public void OnSelectItem(int id)
    {
        foreach (Control ctrl in sub_research_panel_types)
            ctrl.Visible = false;

        ItemInfo info = researchable_items[id];
        current_selected_research_info = info;
        current_research = Database.researchs[info.id];

        current_research_level = 0;
        if (research_saves.ContainsKey(info.id))
            current_research_level = research_saves[info.id].research_level;

        if (current_research_level >= current_research.item_research_levels.Count)
        {
            full_researched_panel.Visible = true;
            return;
        }

        int type = GD.RandRange(0, sub_research_panel_types.Count - 1);
        sub_research_panel_types[type].Visible = true;
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
        }

        foreach (Control c in item_row_manager.GetChildren())
            c.QueueFree();

        research_title.Text = "Title MAIN: " + id;
        research_description.Text = "Description MAIN: " + id;
    }

    private void SetText(ItemInfo info)
    {
        Inventory.ITEM_ID id = info.id;

        if (!Database.researchs.ContainsKey(id))
            return;

        for (int i = 1; i < Database.researchs[id].item_research_levels.Count + 1; i++)
        {
            if (research_saves.ContainsKey(id))
                if (research_saves[id].research_level >= i)
                    continue;

            LevelTab lt = research_level_tab.Instantiate() as LevelTab;
            //tab_container.AddChild(lt);
            lt.UpdateLevelTab(
                Database.researchs[id].translation_string,
                i,
                Database.researchs[id].item_research_levels[i - 1], //Bonus String
                (Database.UPGRADE_LEVEL)i
            );
        }
    }

    public void OnSelectSubResearch(int id)
    {
        selected_id = id;
        research_title.Text = "SUB Title: " + id;
        research_description.Text = "SUB Description: " + id;
        item_row_manager.SetResourcesOnUI(
            new Array<Item> { new Item(current_selected_research_info, 5) }
        );
    }

    public void OnResearchButton()
    {
        if (selected_id == -1)
            return;

        research_activ_rect.Visible = true;
        research_current_title.Text = current_research
            .item_research_levels[current_research_level]
            .sub_levels[selected_id]
            .category.ToString();
        research_current_progress.Value = 0;
        research_current_progress.MaxValue = current_research
            .item_research_levels[current_research_level]
            .sub_levels[selected_id]
            .time_needed;
        timer.Start();
    }

    public void OnTimerTimeout()
    {
        if (research_current_progress.Value < research_current_progress.MaxValue)
        {
            research_current_progress.Value += 1;
            research_current_time_left.Text =
                "Time: "
                + research_current_progress.Value.ToString()
                + " / "
                + research_current_progress.MaxValue.ToString();
        }
        else
        {
            timer.Stop();
            OnResearchFinished();
        }
    }

    public void OnSubResearchSelectButton()
    {
        GD.Print("Sub Research Select Button");
    }

    public void OnResearchFinished()
    {
        //ItemInfo info = research_slot.GetSlotItemUI().item.info;
        Inventory.ITEM_ID id = current_selected_research_info.id;
        UpdateSelectTab();

        if (!research_saves.ContainsKey(id))
            research_saves[id] = new ResearchSave();
        else if (
            research_saves[current_selected_research_info.id].research_level
            >= Database.researchs[current_selected_research_info.id].item_research_levels.Count
        )
        {
            full_researched_panel.Visible = true;
        }

        research_activ_rect.Visible = false;
        Research_Points += 1;
    }

    public void ClearResearchPanel() { }
}
