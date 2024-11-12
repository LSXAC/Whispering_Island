using System;
using System.Diagnostics;
using Godot;

public partial class ResourceObject : Building_Node
{
    //@todo: Animation for each Click
    //@todo: Remove Animations

    [Export]
    public AnimationPlayer anim_player;

    [Export]
    private int max_durability = 3;

    [Export]
    private int mining_amount = 1;

    [Export]
    private int respawn_seconds = 60;

    [Export]
    private float click_cooldown_time = 0.5f;

    [Export]
    private int mining_amount_last = 3;

    [Export]
    private ItemInfo item_info;

    [Export]
    public GpuParticles2D gpu_particles;
    private int current_durability;

    TimerBar timer_bar;
    Area2D interactableArea;
    public bool in_cooldown = false;

    public override void _Ready()
    {
        current_durability = max_durability;
        interactableArea = GetNode<Area2D>("MouseArea");

        if (HasNode("TimerBar"))
        {
            timer_bar = GetNode<TimerBar>("TimerBar");
            timer_bar.parent = this;
        }

        if (timer_bar == null)
            GD.PrintErr("ResourceObject: " + this.Name + " | doesn't have TimerBar");
        anim_player.PlayBackwards("Break");
    }

    public override void _Process(double delta)
    {
        if (!Game_Manager.tutorial_finished && Visible)
            Visible = false;
        else if (Game_Manager.tutorial_finished && !Visible)
            Visible = true;
    }

    public void ResourceObjectLoad(ResourceObjectSave ros)
    {
        if (ros == null)
            return;

        current_durability = ros.current_durability;
        in_cooldown = ros.in_cooldown;
        if (ros.last_state != TimerBar.state.NONE)
            if (ros.time_left == 0)
                Reset(from_loading: true);
            else
                StartTimerBar(ros.last_state, ros.time_left, from_loading: true);
    }

    public ResourceObjectSave SaveResourceObject()
    {
        ResourceObjectSave ros = new ResourceObjectSave(
            in_cooldown: in_cooldown,
            last_state: timer_bar.currentstate,
            (int)timer_bar.Value,
            current_durability
        );
        return ros;
    }

    public override void OnMouseClick()
    {
        Debug.Print(Name + " | " + in_cooldown);
        if (GlobalFunctions.GetDistanceToPlayer(this.GlobalPosition) >= 40f)
            return;
        if (in_cooldown)
            return;

        Hit();
    }

    private void Hit()
    {
        Player.INSTANCE.player_stats.AddFatigue(0.25f);
        current_durability--;
        gpu_particles.Emitting = true;
        if (current_durability == 0)
        {
            anim_player.Play("Break");
            player_ui.AddItemLabelUI(
                "Cleared: +"
                    + mining_amount_last
                    + " "
                    + TranslationServer.Translate(item_info.item_name.ToString())
            );
            StartTimerBar(TimerBar.state.RESPAWNING, respawn_seconds);
            Inventory.INSTANCE.AddItem(item_info, 3, Inventory.INSTANCE.inventory_items);
            GetNode<CollisionShape2D>("Collision").Disabled = true;
            GetNode<Sprite2D>("Shadow").Visible = false;
            hover_menu.DisableHoverMenu();
            return;
        }
        anim_player.Play("Hit");
        StartTimerBar(TimerBar.state.COOLDOWN, click_cooldown_time);
        player_ui.AddItemLabelUI(
            "+" + mining_amount + " " + TranslationServer.Translate(item_info.item_name.ToString())
        );
        Inventory.INSTANCE.AddItem(item_info, mining_amount, Inventory.INSTANCE.inventory_items);
    }

    private void StartTimerBar(TimerBar.state state, double time, bool from_loading = false)
    {
        if (state == TimerBar.state.RESPAWNING && from_loading)
            anim_player.Play("Respawning");

        timer_bar.InitTimer(max_seconds: time, new_state: state);
        in_cooldown = true;
    }

    public void Reset(bool from_loading = false)
    {
        if (timer_bar.currentstate == TimerBar.state.RESPAWNING || from_loading)
        {
            current_durability = max_durability;
            anim_player.PlayBackwards("Break");
        }
        GetNode<CollisionShape2D>("Collision").Disabled = false;
        GetNode<Sprite2D>("Shadow").Visible = true;
        in_cooldown = false;
        interactableArea.Monitoring = true;
        timer_bar.currentstate = TimerBar.state.COOLDOWN;
    }
}
