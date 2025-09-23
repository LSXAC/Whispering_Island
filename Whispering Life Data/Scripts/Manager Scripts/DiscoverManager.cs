using System;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;

public partial class DiscoverManager : Control
{
    [Export]
    public AnimationPlayer anim;

    [Export]
    public Control discovery_panel;

    [Export]
    public Label title,
        type;

    [Export]
    public TextureRect item_icon;
    public static DiscoverManager instance = null;
    public static Dictionary<Inventory.ITEM_ID, bool> discovered_items =
        new Dictionary<Inventory.ITEM_ID, bool>();

    bool in_close = false;

    public override void _Ready()
    {
        discovered_items = new Dictionary<Inventory.ITEM_ID, bool>();
        instance = this;
        discovery_panel.Visible = false;
    }

    public void StartDiscovery(ItemInfo info)
    {
        if (info == null)
            return;

        if (IsDiscovered(info.id))
            return;

        discovered_items[info.id] = true;
        type.Text = info.name;
        item_icon.Texture = info.texture;
        GameManager.In_Cutscene = true;
        discovery_panel.Visible = true;
        anim.SpeedScale = 2f;
        anim.Play("Start_Discovery");
    }

    public bool IsDiscovered(Inventory.ITEM_ID id)
    {
        if (discovered_items.ContainsKey(id))
        {
            return discovered_items[id];
        }
        return false;
    }

    public void StopDiscovery()
    {
        anim.SpeedScale = 3f;
        in_close = true;
        anim.PlayBackwards("Start_Discovery");
    }

    public void OnAnimationFinished(string anim_name)
    {
        if (!in_close)
            return;

        if (anim_name == "Start_Discovery")
        {
            GameManager.In_Cutscene = false;
            discovery_panel.Visible = false;
            in_close = false;
        }
    }

    public void OnButton()
    {
        StopDiscovery();
    }
}
