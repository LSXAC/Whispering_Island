using System;
using System.Diagnostics;
using Godot;

public partial class MineableObject : placeable_building
{
    [Export]
    public Node2D hit_point;

    [Export]
    public PlayerStats.TYPE type;

    [Export]
    public MINING_LEVEL mining_level = MINING_LEVEL.Hand;

    [Export]
    public int max_durability = 3;

    [Export]
    private int respawn_seconds = 60;

    [Export]
    private float click_cooldown_time = 0.5f;

    [Export]
    private int mining_bonus = 2;

    [Export]
    private ItemInfo item_resource;

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

    public enum MINING_LEVEL
    {
        Hand,
        Wood,
        Stone,
        Mystic,
        Iron,
    }

    private PackedScene hit_label = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/hit_label.tscn"
    );

    [Signal]
    public delegate void ReadyFinishedEventHandler();

    public override void _Ready()
    {
        base._Ready();
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
        //sprite_anim_player.animation_player.PlayBackwards("Break");
        StartTimerBar(TimerBar.STATE.SPAWNING, respawn_seconds);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!GameManager.instance.tutorial_finished && Visible)
            Visible = false;
        else if (GameManager.instance.tutorial_finished && !Visible)
            Visible = true;
    }

    public void ResourceObjectLoad(ResourceObjectSave ros)
    {
        if (ros == null)
            return;

        Position = ros.position;
        current_durability = ros.current_durability;
        in_cooldown = ros.in_cooldown;
        if (ros.last_state != TimerBar.STATE.NONE)
            if (ros.time_left == 0)
                Reset(from_loading: true);
            else
                StartTimerBar(ros.last_state, ros.time_left, from_loading: true);
    }

    public ResourceObjectSave SaveResourceObject()
    {
        ResourceObjectSave ros = new ResourceObjectSave(
            in_cooldown: in_cooldown,
            last_state: timer_bar.current_state,
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
        try
        {
            Hit();
        }
        catch (Exception e)
        {
            GD.PrintErr("Error while hitting MineableObject: " + e.Message);
            PlayerUI.AddItemLabelUI("ERROR: " + e.Message);
        }
    }

    private void Hit()
    {
        if (!PlayerUI.instance.equipmentSelectBar.HasSameUseType(type))
        {
            PlayerUI.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_WRONG_TOOL"));
            return;
        }

        if (PlayerUI.instance.equipmentSelectBar.GetSelectedTypeLevel() < mining_level)
        {
            PlayerUI.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_WEAK_TYPE_LEVEL"));
            return;
        }

        //Check if Item can be in PlayerInventoryUI
        if ((current_durability - CalculateMiningAmountInt()) > 0)
        {
            if (
                !PlayerInventoryUI.instance.CanReceiveItem(
                    new Item(item_resource, CalculateMiningAmountInt()),
                    PlayerInventoryUI.instance.inventory_items
                )
            )
            {
                PlayerUI.AddItemLabelUI(
                    TranslationServer.Translate("PLAYERUI_PlayerInventoryUI_FULL")
                );
                return;
            }
        }
        else
        {
            int amount = (int)(mining_bonus * CalculateMiningAmountInt()) + current_durability;
            if (
                !PlayerInventoryUI.instance.CanReceiveItem(
                    new Item(item_resource, amount),
                    PlayerInventoryUI.instance.inventory_items
                )
            )
            {
                PlayerUI.AddItemLabelUI(
                    TranslationServer.Translate("PLAYERUI_PlayerInventoryUI_FULL")
                );
                return;
            }
            if (drops_extra_item)
            {
                if (
                    !PlayerInventoryUI.instance.CanReceiveItem(
                        new Item(extra_item_info, extra_item_amount),
                        PlayerInventoryUI.instance.inventory_items
                    )
                )
                {
                    PlayerUI.AddItemLabelUI(
                        TranslationServer.Translate("PLAYERUI_PlayerInventoryUI_FULL")
                    );
                    return;
                }
            }
        }

        if (hit_point != null)
        {
            CharacterBody2D hit_lab = hit_label.Instantiate() as CharacterBody2D;
            int time = rnd.Next(-8, 9);
            hit_lab.GetChild<HitLabel>(0).InitText("- " + CalculateMiningAmountInt());
            IslandManager
                .instance.GetNearestIsland(GetGlobalMousePosition())
                .island_object_save_manager.AddChild(hit_lab);

            hit_lab.GlobalPosition = hit_point.GlobalPosition - new Vector2(11, 10);
            hit_lab.Position += new Vector2(time, 0);
        }

        Player.instance.player_stats.AddFatigue(0.25f);
        current_durability -= CalculateMiningAmountInt();

        //Selected Item Durability + Break -------------------------------------------------------------
        if (PlayerUI.instance.equipmentSelectBar.GetSelectedSlotItemUI() != null)
        {
            SlotItemUI slot_item_ui = PlayerUI.instance.equipmentSelectBar.GetSelectedSlotItemUI();
            if (slot_item_ui.item.info.HasAttribute<ToolAttribute>())
            {
                EquipmentPanel
                    .instance.slots_tool[EquipmentSelectBar.current_selected_slot]
                    .GetSlotItemUI()
                    .current_durability -= CalculateMiningAmountInt();
                EquipmentPanel.UpdateSlotDurability(EquipmentSelectBar.current_selected_slot);
            }
        }
        //-----------------------------------------------------------------------------------------------------

        gpu_particles.Emitting = true;
        if (current_durability <= 0)
        {
            //sprite_anim_player.animation_player.Play("Break");
            int amount = (int)(mining_bonus * CalculateMiningAmountInt()) + current_durability;
            PlayerUI.AddItemLabelUI(
                "Bonus: +"
                    + amount
                    + " "
                    + TranslationServer.Translate(item_resource.name.ToString())
            );
            PlayerInventoryUI.instance.AddItem(
                new Item(item_resource, amount),
                PlayerInventoryUI.instance.inventory_items
            );

            if (drops_extra_item)
            {
                PlayerUI.AddItemLabelUI(
                    "Bonus: +"
                        + extra_item_amount
                        + " "
                        + TranslationServer.Translate(extra_item_info.name.ToString())
                );
                PlayerInventoryUI.instance.AddItem(
                    new Item(extra_item_info, extra_item_amount),
                    PlayerInventoryUI.instance.inventory_items
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

        //sprite_anim_player.animation_player.Play("Hit");
        StartTimerBar(TimerBar.STATE.COOLDOWN, click_cooldown_time);
        PlayerUI.AddItemLabelUI(
            "+"
                + CalculateMiningAmountInt()
                + " "
                + TranslationServer.Translate(item_resource.name.ToString())
        );
        PlayerInventoryUI.instance.AddItem(
            new Item(item_resource, CalculateMiningAmountInt()),
            PlayerInventoryUI.instance.inventory_items
        );

        hover_menu.InitHoverMenu(this);
    }

    public int CalculateMiningAmountInt()
    {
        return (int)(1 * Skilltree.GetSkillProgress(Skilltree.SKILLTYPE.HIT));
    }

    private void StartTimerBar(TimerBar.STATE state, double time, bool from_loading = false)
    {
        //if (state == TimerBar.STATE.SPAWNING && from_loading)
        //   sprite_anim_player.animation_player.PlayBackwards("Break");

        if (state == TimerBar.STATE.SPAWNING)
            if (collision_shape != null)
                collision_shape.Disabled = false;

        timer_bar.InitTimer(max_seconds: time, new_state: state);
        in_cooldown = true;
    }

    public void Reset(bool from_loading = false)
    {
        if (timer_bar.current_state == TimerBar.STATE.SPAWNING || from_loading)
            current_durability = max_durability;

        if (HasNode("Shadow"))
            GetNode<Sprite2D>("Shadow").Visible = true;

        in_cooldown = false;
        interactableArea.Monitoring = true;
        timer_bar.current_state = TimerBar.STATE.NONE;
    }
}
