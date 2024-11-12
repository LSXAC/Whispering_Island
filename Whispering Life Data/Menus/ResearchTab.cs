using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    [Export]
    public Slot research_slot;

    public PackedScene research_level_tab = ResourceLoader.Load<PackedScene>(
        "res://Prefabs/Research_Level_Tab.tscn"
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
        UpdateLevelTabs();
    }

    public void UpdateLevelTabs()
    {
        if (research_slot_item != null)
        {
            research_slot.SetItem(
                Inventory.INSTANCE.item_Types[research_slot_item.item_id],
                research_slot_item.amount
            );
            SetText();
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

    private void SetText()
    {
        item_name_label.Text =
            ""
            + TranslationServer.Translate(
                Inventory.INSTANCE.item_Types[research_slot_item.item_id].item_name
            );
        item_level_label.Text = "-/-";
    }
}
