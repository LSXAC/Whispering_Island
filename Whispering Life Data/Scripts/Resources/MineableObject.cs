using System;
using System.Diagnostics;
using Godot;

public partial class MineableObject : placeable_building
{
    [Export]
    public AnimationPlayer anim_player;

    [Export]
    public Node2D hit_point;

    [Export]
    public StatsPanel.stat_types type;

    [Export]
    public ItemInfo.Type_Level type_level = ItemInfo.Type_Level.Hand;

    [Export]
    public int max_durability = 3;

    [Export]
    private int respawn_seconds = 60;

    [Export]
    private float click_cooldown_time = 0.5f;

    [Export]
    private int mining_bonus = 2;

    [Export]
    private ItemInfo item_info;

    [Export]
    public bool drops_extra_item;

    [Export]
    public int extra_item_amount = 0;

    [Export]
    public ItemInfo extra_item_info;

    [Export]
    public GpuParticles2D gpu_particles;
    public int current_durability;

    TimerBar timer_bar;
    Area2D interactableArea;
    public bool in_cooldown = false;
    Random rnd = new Random();

    private PackedScene hit_label = ResourceLoader.Load<PackedScene>(
        "res://Prefabs/hit_label.tscn"
    );

    [Signal]
    public delegate void ReadyFinishedEventHandler();

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
    }

    public void SpawnPlant()
    {
        anim_player.PlayBackwards("Break");
        StartTimerBar(TimerBar.state.SPAWNING, respawn_seconds);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Game_Manager.INSTANCE.tutorial_finished && Visible)
            Visible = false;
        else if (Game_Manager.INSTANCE.tutorial_finished && !Visible)
            Visible = true;
    }

    public void ResourceObjectLoad(ResourceObjectSave ros)
    {
        if (ros == null)
            return;

        Position = ros.position;
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
            current_durability,
            Position,
            building_id
        );
        return ros;
    }

    public override void OnMouseClick()
    {
        if (!CheckClickDependencies(this))
            return;

        if (in_cooldown)
            return;

        Hit();
    }

    private void Hit()
    {
        if (!player_ui.INSTANCE.equipmentSelectBar.HasSameUseType(type))
        {
            player_ui.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_WRONG_TOOL"));
            return;
        }

        if (player_ui.INSTANCE.equipmentSelectBar.GetSelectedTypeLevel() < type_level)
        {
            player_ui.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_WEAK_TYPE_LEVEL"));
            return;
        }

        //Check if Item can be in Inventory
        if (
            (
                current_durability
                - (int)(
                    Player.INSTANCE.player_stats.stat_amounts[(int)type]
                    * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT)
                )
            ) > 0
        )
        {
            if (
                !Inventory.INSTANCE.CanReceiveItem(
                    item_info,
                    Inventory.INSTANCE.inventory_items,
                    (int)(
                        Player.INSTANCE.player_stats.stat_amounts[(int)type]
                        * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT)
                    )
                )
            )
            {
                player_ui.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_INVENTORY_FULL"));
                return;
            }
        }
        else
        {
            int amount =
                (int)(
                    mining_bonus
                    * Player.INSTANCE.player_stats.stat_amounts[(int)type]
                    * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT)
                ) + current_durability;
            if (
                !Inventory.INSTANCE.CanReceiveItem(
                    item_info,
                    Inventory.INSTANCE.inventory_items,
                    amount
                )
            )
            {
                player_ui.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_INVENTORY_FULL"));
                return;
            }
            if (drops_extra_item)
            {
                if (
                    !Inventory.INSTANCE.CanReceiveItem(
                        extra_item_info,
                        Inventory.INSTANCE.inventory_items,
                        extra_item_amount
                    )
                )
                {
                    player_ui.AddItemLabelUI(
                        TranslationServer.Translate("PLAYERUI_INVENTORY_FULL")
                    );
                    return;
                }
            }
        }

        if (hit_point != null)
        {
            CharacterBody2D hit_lab = hit_label.Instantiate() as CharacterBody2D;
            int time = rnd.Next(-8, 9);
            hit_lab
                .GetChild<HitLabel>(0)
                .InitText(
                    "- "
                        + (int)(
                            Player.INSTANCE.player_stats.stat_amounts[(int)type]
                            * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT)
                        )
                );
            Islands_Manager
                .INSTANCE.GetNearestIsland(GetGlobalMousePosition())
                .building_manager.AddChild(hit_lab);

            hit_lab.GlobalPosition = hit_point.GlobalPosition - new Vector2(11, 10);
            hit_lab.Position += new Vector2(time, 0);
        }

        Player.INSTANCE.player_stats.AddFatigue(0.25f);
        current_durability -= (int)(
            Player.INSTANCE.player_stats.stat_amounts[(int)type]
            * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT)
        );

        //Selected Item Durability + Break -------------------------------------------------------------
        if (player_ui.INSTANCE.equipmentSelectBar.GetSelectedItem() != null)
            if (player_ui.INSTANCE.equipmentSelectBar.GetSelectedItem().item_info.has_durability)
            {
                EquipmentPanel
                    .INSTANCE.slots_tool[EquipmentSelectBar.current_selected_slot]
                    .GetItem()
                    .current_durability -= (int)(
                    Player.INSTANCE.player_stats.stat_amounts[(int)type]
                    * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT)
                );
                EquipmentPanel.UpdateSlotDurability(EquipmentSelectBar.current_selected_slot);
            }
        //-----------------------------------------------------------------------------------------------------

        gpu_particles.Emitting = true;
        if (current_durability <= 0)
        {
            anim_player.Play("Break");
            int amount =
                (int)(
                    mining_bonus
                    * Player.INSTANCE.player_stats.stat_amounts[(int)type]
                    * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT)
                ) + current_durability;
            player_ui.AddItemLabelUI(
                "Bonus: +"
                    + amount
                    + " "
                    + TranslationServer.Translate(item_info.item_name.ToString())
            );
            Inventory.INSTANCE.AddItem(item_info, amount, Inventory.INSTANCE.inventory_items);

            if (drops_extra_item)
            {
                player_ui.AddItemLabelUI(
                    "Bonus: +"
                        + extra_item_amount
                        + " "
                        + TranslationServer.Translate(extra_item_info.item_name.ToString())
                );
                Inventory.INSTANCE.AddItem(
                    extra_item_info,
                    extra_item_amount,
                    Inventory.INSTANCE.inventory_items
                );
            }

            if (HasNode("Collision"))
                GetNode<CollisionShape2D>("Collision").Disabled = true;
            if (HasNode("Shadow"))
                GetNode<Sprite2D>("Shadow").Visible = false;

            hover_menu.DisableHoverMenu();
            QueueFree();
            return;
        }

        anim_player.Play("Hit");
        StartTimerBar(TimerBar.state.COOLDOWN, click_cooldown_time);
        player_ui.AddItemLabelUI(
            "+"
                + (
                    (int)(
                        Player.INSTANCE.player_stats.stat_amounts[(int)type]
                        * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT)
                    )
                )
                + " "
                + TranslationServer.Translate(item_info.item_name.ToString())
        );
        Inventory.INSTANCE.AddItem(
            item_info,
            (int)(
                Player.INSTANCE.player_stats.stat_amounts[(int)type]
                * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT)
            ),
            Inventory.INSTANCE.inventory_items
        );

        hover_menu.InitHoverMenu(this);
    }

    private void StartTimerBar(TimerBar.state state, double time, bool from_loading = false)
    {
        if (state == TimerBar.state.SPAWNING && from_loading)
            anim_player.PlayBackwards("Break");

        if (state == TimerBar.state.SPAWNING)
            if (collision_shape != null)
                collision_shape.Disabled = false;

        timer_bar.InitTimer(max_seconds: time, new_state: state);
        in_cooldown = true;
    }

    public void Reset(bool from_loading = false)
    {
        if (timer_bar.currentstate == TimerBar.state.SPAWNING || from_loading)
            current_durability = max_durability;

        if (HasNode("Shadow"))
            GetNode<Sprite2D>("Shadow").Visible = true;

        in_cooldown = false;
        interactableArea.Monitoring = true;
        timer_bar.currentstate = TimerBar.state.NONE;
    }
}
