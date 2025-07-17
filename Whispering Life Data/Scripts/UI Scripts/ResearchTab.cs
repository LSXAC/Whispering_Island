using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class ResearchTab : ColorRect
{
    public static Dictionary<Inventory.ITEM_ID, ResearchSave> research_saves =
        new Dictionary<Inventory.ITEM_ID, ResearchSave>();

    [Export]
    public TabContainer tab_container;

    [Export]
    public Label item_name_label;

    [Export]
    public Label item_level_label;

    [Export]
    public Slot research_slot;

    [Export]
    public ProgressBar progressBar;

    [Export]
    public Timer timer;
    public PackedScene research_level_tab = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/Research_Level_Tab.tscn"
    );

    public Array<ResearchSave> research_states = new Array<ResearchSave>();
    public static ItemSave research_slot_item = null;

    public static ResearchTab instance = null;

    [Export]
    public ColorRect working_panel;

    [Export]
    public int Research_Points = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Research_Points = 0;
        instance = this;
        ClearText();
    }

    public void OnVisiblityChanged()
    {
        UpdateLevelTabs();
    }

    public void UpdateLevelTabs()
    {
        if (research_slot_item != null)
        {
            research_slot.SetItem(
                new Item(
                    Inventory.ITEM_TYPES[(Inventory.ITEM_ID)research_slot_item.item_id],
                    research_slot_item.amount
                )
            );
            SetText(research_slot.GetSlotItemUI().item.info);
        }
        else
        {
            ClearText();
            research_slot.ClearSlotItem();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private void ClearText()
    {
        item_name_label.Text = TranslationServer.Translate("RESEARCH_NO_ITEM");
        item_level_label.Text = "-/-";

        foreach (LevelTab lts in tab_container.GetChildren())
            lts.QueueFree();
    }

    private void SetText(ItemInfo info)
    {
        foreach (LevelTab lts in tab_container.GetChildren())
            lts.QueueFree();

        Inventory.ITEM_ID id = info.id;

        if (!Database.researchs.ContainsKey(id))
            return;

        for (int i = 1; i < Database.researchs[id].item_research_levels.Count + 1; i++)
        {
            if (research_saves.ContainsKey(id))
                if (research_saves[id].research_level >= i)
                    continue;

            LevelTab lt = research_level_tab.Instantiate() as LevelTab;
            tab_container.AddChild(lt);
            lt.UpdateLevelTab(
                Database.researchs[id].translation_string,
                i,
                Database.researchs[id].item_research_levels[i - 1], //Bonus String
                (Database.UPGRADE_LEVEL)i
            );
        }
        item_name_label.Text =
            ""
            + TranslationServer.Translate(
                Inventory.ITEM_TYPES[(Inventory.ITEM_ID)research_slot_item.item_id].name
            );
        if (research_saves.ContainsKey(id))
            item_level_label.Text =
                TranslationServer.Translate("RESEARCH_RESEARCHLEVEL")
                + ": "
                + research_saves[id].research_level;
        else
            item_level_label.Text = TranslationServer.Translate("RESEARCH_RESEARCHLEVEL") + ": 0";
    }

    public void OnResearchButton()
    {
        if (research_slot_item == null)
            return;

        if (tab_container.CurrentTab != 0)
            return;
        //Remove Items, for Prototype, researching is only time consuming

        progressBar.Value = 0;
        progressBar.MaxValue = 100 * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.RESEARCH_TIME);
        working_panel.Visible = true;
        timer.Start();
    }

    public void OnTimerTimeout()
    {
        progressBar.Value += 0.1;
        if (progressBar.Value >= progressBar.MaxValue)
        {
            timer.Stop();
            OnResearchFinished();
            progressBar.Value = 0;
        }
    }

    public void OnResearchFinished()
    {
        ItemInfo info = research_slot.GetSlotItemUI().item.info;
        Inventory.ITEM_ID id = info.id;

        Debug.Print(tab_container.CurrentTab.ToString());

        if (!research_saves.ContainsKey(id))
            research_saves[id] = new ResearchSave();

        if (
            research_saves[id]
                .AddLevel(tab_container.GetChild<LevelTab>(tab_container.CurrentTab).level + 1)
        )
            Debug.Print(
                "Level" + tab_container.CurrentTab + " added to " + research_saves[id].ToString()
            );
        Research_Points += 1;
        Debug.Print(research_saves[id].research_level + " current level <:");
        SetText(research_slot.GetSlotItemUI().item.info);
        tab_container.GetChild(0).QueueFree();
        working_panel.Visible = false;
    }
}
