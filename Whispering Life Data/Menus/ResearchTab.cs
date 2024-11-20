using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class ResearchTab : ColorRect
{
    [Export]
    public Dictionary<InventoryBase.ITEM_ID, ResearchSave> research_saves;

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
        "res://Prefabs/Research_Level_Tab.tscn"
    );

    public Array<ResearchSave> research_states = new Array<ResearchSave>();
    public ItemSave research_slot_item = null;

    public static ResearchTab INSTANCE = null;

    [Export]
    public ColorRect working_panel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        INSTANCE = this;
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
                Inventory.INSTANCE.item_Types[(InventoryBase.ITEM_ID)research_slot_item.item_id],
                research_slot_item.amount
            );
            SetText(research_slot.GetItem().item_info);
        }
        else
        {
            ClearText();
            research_slot.ClearItem();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private void ClearText()
    {
        item_name_label.Text = "No Item in Research Slot";
        item_level_label.Text = "-/-";
    }

    private void SetText(ItemInfo item_info)
    {
        foreach (LevelTab lt in tab_container.GetChildren())
            lt.QueueFree();

        InventoryBase.ITEM_ID id = item_info.unique_id;

        if (!Database.researchs.ContainsKey(id))
            return;

        for (int i = 0; i < Database.researchs[id].research_levels.Count; i++)
        {
            if (research_saves.ContainsKey(id))
                if (research_saves[id].research_level >= i)
                    continue;

            LevelTab lt = research_level_tab.Instantiate() as LevelTab;
            tab_container.AddChild(lt);
            lt.UpdateLevelTab(
                Database.researchs[id].translation_string,
                i,
                "Empty",
                (Database.UPGRADE_LEVEL)i
            );
        }
        item_name_label.Text =
            ""
            + TranslationServer.Translate(
                Inventory
                    .INSTANCE
                    .item_Types[(InventoryBase.ITEM_ID)research_slot_item.item_id]
                    .item_name
            );
        if (research_saves.ContainsKey(id))
            item_level_label.Text = "Research Level: " + research_saves[id].research_level;
        else
            item_level_label.Text = "No Researches";

        //Update Level Tab Box
    }

    public void OnResearchButton()
    {
        if (research_slot_item == null)
            return;
        if (tab_container.CurrentTab == -1)
            return;

        //Remove Items
        //start Research
        progressBar.Value = 0;
        working_panel.Visible = true;
        timer.Start();
    }

    public void OnTimerTimeout()
    {
        progressBar.Value += 0.1;
        if (progressBar.Value >= 100)
        {
            timer.Stop();
            OnResearchFinished();
            progressBar.Value = 0;
        }
    }

    public void OnResearchFinished()
    {
        ItemInfo item_info = research_slot.GetItem().item_info;
        InventoryBase.ITEM_ID id = item_info.unique_id;

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

        Debug.Print(research_saves[id].research_level + " current level <:");
        SetText(research_slot.GetItem().item_info);
        working_panel.Visible = false;
    }
}
