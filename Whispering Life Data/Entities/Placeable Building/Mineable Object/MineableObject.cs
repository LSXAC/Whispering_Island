using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class MineableObject : placeable_building
{
    [Export]
    public Node2D hit_point;

    [Export]
    public bool rnd_height = false;

    [Export]
    public bool growable = false;

    [Export]
    public PlayerStats.TOOLTYPE tool_type;

    [Export]
    public int variant = 0;

    [Export]
    public MINING_LEVEL mining_level = MINING_LEVEL.HAND;
    public Texture2DArray mine_textures = null;
    public Texture2DArray growth_textures = null;

    [Export]
    public Node2D variants_parent;

    [Export]
    public Node2D growth_variants_parent;

    [Export]
    private int respawn_seconds = 60;

    [Export]
    private float click_cooldown_time = 0.5f;

    [Export]
    public Item resource_item;

    [Export]
    public Item extra_item;

    [Export]
    public float item_amount_multiplicator = 2.0f;

    [Export]
    public GpuParticles2D gpu_particles;
    public int max_durability;
    public int current_durability;

    TimerBar timer_bar;
    Area2D interactableArea;
    public static bool in_cooldown = false;
    HitLabelManager hitLabelManager;

    public enum MINING_LEVEL
    {
        HAND,
        WOOD,
        STONE,
        IRON,
        OBSIDIAN,
        MYSTIC,
    }

    protected override bool SupportsHoldAction => true;

    [Signal]
    public delegate void ReadyFinishedEventHandler();

    public void GetTexture2DArray(int variant)
    {
        if (variants_parent == null)
        {
            GD.PrintErr("variants_parent is null in GetTexture2DArray");
            return;
        }

        int childCount = variants_parent.GetChildCount();
        if (variant < 0 || variant >= childCount)
        {
            GD.PrintErr(
                $"Variant index {variant} is out of bounds. Available variants: {childCount}"
            );
            variant = 0;
        }

        Texture2DArray t2d_array = variants_parent.GetChild<Texture2DArray>(variant);
        mine_textures = t2d_array;
        if (!growable)
            return;

        if (growth_variants_parent != null)
        {
            int growth_childCount = growth_variants_parent.GetChildCount();
            if (variant < 0 || variant >= growth_childCount)
            {
                GD.PrintErr(
                    $"Growth variant index {variant} is out of bounds. Available growth variants: {growth_childCount}"
                );
                return;
            }
            Texture2DArray growth_t2d_array = growth_variants_parent.GetChild<Texture2DArray>(
                variant
            );
            growth_textures = growth_t2d_array;
        }
    }

    public int GetRandomVariation()
    {
        if (variants_parent == null)
        {
            GD.PrintErr("variants_parent is null in GetRandomVariation");
            variant = 0;
            return variant;
        }

        int childCount = variants_parent.GetChildCount();
        if (childCount <= 0)
        {
            GD.PrintErr("No variants available in variants_parent");
            variant = 0;
            return variant;
        }

        Random rnd = new Random();
        variant = rnd.Next(0, childCount);
        return variant;
    }

    public void SetVariation(int variant, bool growth = false)
    {
        this.variant = variant;
        Debug.Print("Mineable Object variation: " + variant);
        GetTexture2DArray(variant);
        if (growth)
            UpdateGrowthTexture();
    }

    public override void _Ready()
    {
        base._Ready();
        if (rnd_height)
        {
            Random random = new Random();
            float rnd_y = 0.75f + (float)(random.NextDouble() / 2f);
            Scale = new Vector2(rnd_y, rnd_y);
        }

        if (variants_parent == null)
            return;

        GetTexture2DArray(variant);
        max_durability = mine_textures.textures.Count - 1;
        current_durability = max_durability;
        sprite_anim_manager.SetTexture2D(mine_textures.textures[current_durability]);

        collision_polygon.Polygon = mine_textures.polygon_points;
        sprite_anim_manager.shadowNode.SetTexture(
            mine_textures.shadow_textures[current_durability]
        );

        if (Logger.NodeIsNotNull(GetNode<TimerBar>("TimerBar")))
        {
            timer_bar = GetNode<TimerBar>("TimerBar");
            timer_bar.parent = this;
        }

        if (Logger.NodeIsNotNull(GetNode<HitLabelManager>("HitLabelManager")))
            hitLabelManager = GetNode<HitLabelManager>("HitLabelManager");

        interactableArea = GetNode<Area2D>("MouseArea");
        SetResourceTexture();
        Debug.Print("First Mineable");
    }

    public void SpawnPlant()
    {
        UpdateGrowthTexture();
        collision_polygon.Polygon = growth_textures.polygon_points;
        StartTimerBar(TimerBar.STATE.SPAWNING, respawn_seconds);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!GameManager.instance.tutorial_finished && Visible)
            Visible = false;
        else if (GameManager.instance.tutorial_finished && !Visible)
            Visible = true;
    }

    public override void OnMouseClick()
    {
        if (variants_parent == null)
            return;
        if (!CheckClickDependencies(this))
            return;

        if (in_cooldown)
            return;

        if (timer_bar.current_state == TimerBar.STATE.SPAWNING)
            return;

        try
        {
            Hit();
        }
        catch (Exception e)
        {
            GD.PrintErr("Error while hitting MineableObject: " + e.Message);
        }
    }

    private void Hit()
    {
        if (!PlayerUI.instance.equipmentSelectBar.HasSameUseType(tool_type))
        {
            PlayerUI.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_WRONG_TOOL"));
            return;
        }

        if (PlayerUI.instance.equipmentSelectBar.GetSelectedTypeLevel() < mining_level)
        {
            PlayerUI.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_WEAK_TYPE_LEVEL"));
            return;
        }

        PlayerUI.instance.harvest_timer.WaitTime = click_cooldown_time;
        in_cooldown = true;
        PlayerUI.instance.harvest_timer.Start();

        Item item = resource_item.Clone();
        int miningAmount = CalculateMiningAmountInt();
        int durabilityAfterHit = current_durability - miningAmount;
        int bonusAmount = (int)(resource_item.amount * miningAmount);

        item.amount = (int)(
            miningAmount
            * item_amount_multiplicator
            * (int)Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.MINING_AMOUNT)
        );

        if (durabilityAfterHit >= 0)
        {
            if (!CanReceiveItem(item))
                return;
        }
        else
        {
            if (!CanReceiveItem(item))
                return;

            if (extra_item != null && !CanReceiveItem(extra_item))
                return;
        }

        Player.instance.player_stats.AddFatigue(0.25f);
        current_durability -= miningAmount;

        SetResourceTexture();
        hitLabelManager.ShowHitLabel(miningAmount);
        UpdateToolDurability(miningAmount);

        gpu_particles.Emitting = true;

        DiscoverManager.instance.AddDiscovery(resource_item.info);
        if (current_durability < 0)
        {
            HandleBreak(bonusAmount);
            sprite_anim_manager.shadowNode.RemoveShadow();
            return;
        }

        PlayerUI.AddItemLabelMineableUI(item);
        PlayerInventoryUI.instance.AddItem(item, PlayerInventoryUI.instance.inventory_items);

        hover_menu.InitHoverMenu(this);
    }

    private bool CanReceiveItem(Item item)
    {
        if (extra_item != null && item == extra_item)
        {
            if (
                !SeedInventoryUI.instance.CanReceiveItem(
                    item,
                    SeedInventoryUI.instance.inventory_items
                )
            )
            {
                PlayerUI.AddItemLabelUI(
                    TranslationServer.Translate("PLAYERUI_PlayerInventoryUI_FULL")
                );
                return false;
            }
            return true;
        }
        if (
            !PlayerInventoryUI.instance.CanReceiveItem(
                item,
                PlayerInventoryUI.instance.inventory_items
            )
        )
        {
            PlayerUI.AddItemLabelUI(TranslationServer.Translate("PLAYERUI_PlayerInventoryUI_FULL"));
            return false;
        }
        return true;
    }

    private void UpdateToolDurability(int miningAmount)
    {
        var slot_item_ui = PlayerUI.instance.equipmentSelectBar.GetSelectedSlotItemUI();
        if (slot_item_ui != null && slot_item_ui.item.info.HasAttribute<ToolAttribute>())
        {
            EquipmentPanel
                .instance
                .equipped_tools[EquipmentSelectBar.current_selected_slot]
                .current_durability -= miningAmount;
            EquipmentPanel.UpdateSlots();
        }
    }

    private void HandleBreak(int bonusAmount)
    {
        Item item = resource_item.Clone();
        item.amount = (int)(item.amount * item_amount_multiplicator);
        PlayerUI.AddItemLabelMineableBonusItemUI(item);
        PlayerInventoryUI.instance.AddItem(item, PlayerInventoryUI.instance.inventory_items);

        if (extra_item != null)
        {
            PlayerUI.AddItemLabelMineableBonusItemUI(extra_item);
            SeedInventoryUI.instance.AddItem(extra_item, SeedInventoryUI.instance.inventory_items);
        }

        if (HasNode("Collision"))
            GetNode<CollisionShape2D>("Collision").Disabled = true;
        if (HasNode("Shadow"))
            GetNode<Sprite2D>("Shadow").Visible = false;

        hover_menu.DisableHoverMenu();
        QueueFree();
    }

    private void StartTimerBar(TimerBar.STATE state, double time, bool from_loading = false)
    {
        Debug.Print("Starting TimerBar with state: " + state.ToString() + " for time: " + time);
        if (state == TimerBar.STATE.SPAWNING)
        {
            gpu_particles.Emitting = false;
            if (collision_polygon != null)
                collision_polygon.Disabled = false;
            timer_bar.InitTimer(
                max_seconds: time,
                new_state: state,
                "Growth: 0%",
                UpdateGrowthTexture
            );
            return;
        }

        timer_bar.InitTimer(max_seconds: time, new_state: state, "Growth: 0%");
    }

    public void UpdateGrowthTexture()
    {
        double progress_percent = timer_bar.GetProgressPercent();
        int frame_index = (int)(progress_percent * (growth_textures.textures.Count - 1));
        if (frame_index < 0)
            frame_index = 0;
        if (frame_index >= growth_textures.textures.Count)
            frame_index = growth_textures.textures.Count - 1;
        sprite_anim_manager.SetTexture2D(growth_textures.textures[frame_index]);
        sprite_anim_manager.shadowNode.SetTexture(growth_textures.shadow_textures[frame_index]);
    }

    public void Reset(bool from_loading = false)
    {
        if (timer_bar.current_state == TimerBar.STATE.SPAWNING || from_loading)
            current_durability = max_durability;

        gpu_particles.Emitting = true;
        interactableArea.Monitoring = true;
        timer_bar.current_state = TimerBar.STATE.NONE;
    }

    public int CalculateMiningAmountInt()
    {
        return 1 + (int)PlayerUI.instance.equipmentSelectBar.GetSelectedTypeLevel();
    }

    public override ResourceObjectSave Save()
    {
        if (Logger.NodeIsNotNull(GetNode<TimerBar>("TimerBar")))
        {
            timer_bar = GetNode<TimerBar>("TimerBar");
            timer_bar.parent = this;
        }
        return new ResourceObjectSave(
            in_cooldown: in_cooldown,
            last_state: timer_bar.current_state,
            (int)timer_bar.Value,
            current_durability,
            Position,
            building_id,
            variant
        );
    }

    public override void Load(Resource resource)
    {
        if (resource is ResourceObjectSave ros)
        {
            Position = ros.position;
            current_durability = ros.current_durability;
            in_cooldown = ros.in_cooldown;
            variant = ros.variant;
            GetTexture2DArray(variant);
            if (ros.last_state != TimerBar.STATE.NONE)
                if (ros.time_left == 0)
                    Reset(from_loading: true);
                else
                {
                    StartTimerBar(ros.last_state, ros.time_left, from_loading: true);
                    if (ros.last_state == TimerBar.STATE.SPAWNING && growable)
                    {
                        UpdateGrowthTexture();
                        return;
                    }
                }
            SetResourceTexture();
        }
        else
            GD.PrintErr("Wrong Resource for MineableObject", resource.ResourceName);
    }

    public void SetResourceTexture()
    {
        if (current_durability >= 0 && mine_textures != null)
            if (mine_textures.textures.Count > current_durability)
                if (mine_textures.textures[current_durability] != null)
                {
                    SetTextureToSpriteManager(mine_textures.textures[current_durability]);
                    sprite_anim_manager.shadowNode.SetTexture(
                        mine_textures.shadow_textures[current_durability]
                    );
                }
    }
}
