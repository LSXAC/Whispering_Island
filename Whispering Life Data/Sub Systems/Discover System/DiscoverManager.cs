using System;
using System.Collections;
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
    public Timer timer;

    [Export]
    public TextureRect item_icon;
    public System.Collections.Generic.Queue<ItemInfo> discovery_queue =
        new System.Collections.Generic.Queue<ItemInfo>();
    public static DiscoverManager instance = null;
    public static Dictionary<Inventory.ITEM_ID, bool> discovered_items =
        new Dictionary<Inventory.ITEM_ID, bool>();

    bool in_close = false,
        in_discovery = false;

    public override void _Ready()
    {
        discovered_items = new Dictionary<Inventory.ITEM_ID, bool>();
        instance = this;
        discovery_panel.Visible = false;
    }

    public void AddDiscovery(ItemInfo info)
    {
        if (info == null)
            return;

        if (IsDiscovered(info.id))
            return;

        discovery_queue.Enqueue(info);
    }

    public void Start(ItemInfo info)
    {
        if (info == null)
            return;

        if (IsDiscovered(info.id))
            return;

        discovered_items[info.id] = true;
        type.Text = info.name;
        item_icon.Texture = info.texture;
        discovery_panel.Visible = true;
        anim.SpeedScale = 3f;
        anim.Play("Start_Discovery");
        timer.Start();
    }

    public bool IsDiscovered(Inventory.ITEM_ID id)
    {
        if (discovered_items.ContainsKey(id))
        {
            return discovered_items[id];
        }
        return false;
    }

    public void Stop()
    {
        anim.SpeedScale = 4f;
        in_close = true;
        anim.PlayBackwards("Start_Discovery");
    }

    public void OnAnimationFinished(string anim_name)
    {
        if (!in_close)
            return;

        if (anim_name == "Start_Discovery")
        {
            discovery_panel.Visible = false;
            in_close = false;
            in_discovery = false;
        }
    }

    public void OnCheckTimerTimeout()
    {
        if (in_discovery || discovery_queue.Count == 0)
            return;

        in_discovery = true;
        Start(discovery_queue.Dequeue());
    }

    public void OnTimerTimeout()
    {
        Stop();
    }
}
