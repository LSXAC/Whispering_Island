using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class ResearchTab : ColorRect
{
    [Export]
    public TabContainer tab_container;

    [Export]
    public Label item_name_label;

    [Export]
    public Label item_level_label;

    public PackedScene research_level_tab = ResourceLoader.Load<PackedScene>(
        "Research_Level_Tab.tscn"
    );

    public Array<ResearchSave> research_states = new Array<ResearchSave>();
    public ItemSave research_slot_item = null;

    public static ResearchTab INSTANCE = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        INSTANCE = this;
        ClearText();
    }

    public void OnVisiblityChanged()
    {
        if (research_slot_item == null)
            ClearText();
        else
        {
            UpdateLevelTabs(Inventory.INSTANCE.item_Types[research_slot_item.item_id]);
        }
    }

    public void UpdateLevelTabs(ItemInfo ii) { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private void ClearText()
    {
        item_level_label.Text = "No Item in Research Slot";
        item_level_label.Text = "-/-";
    }
}
