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
    public Array<Texture2D> mine_textures = null;
    public Array<Texture2D> growth_textures = null;

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
    public GpuParticles2D gpu_particles;
    public int max_durability;
    public int current_durability;

    TimerBar timer_bar;
    Area2D interactableArea;
    public bool in_cooldown = false;

    public enum MINING_LEVEL
    {
        HAND,
        WOOD,
        STONE,
        IRON,
        OBSIDIAN,
        MYSTIC,
    }

    private PackedScene hit_label = ResourceLoader.Load<PackedScene>(
        "res://Scenes/UI/hit_label.tscn"
    );

    [Signal]
    public delegate void ReadyFinishedEventHandler();

    public void GetTexture2DArray(int variant)
    {
        Texture2DArray t2d_array = variants_parent.GetChild<Texture2DArray>(variant);
        mine_textures = t2d_array.textures;
        if (!growable)
            return;
        Texture2DArray growth_t2d_array = growth_variants_parent.GetChild<Texture2DArray>(variant);
        growth_textures = growth_t2d_array.textures;
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
        max_durability = mine_textures.Count - 1;
        current_durability = max_durability;
        sprite_anim_manager.SetTexture2D(mine_textures[current_durability]);
        if (Logger.NodeIsNotNull(GetNode<TimerBar>("TimerBar")))
        {
            timer_bar = GetNode<TimerBar>("TimerBar");
            timer_bar.parent = this;
        }
        interactableArea = GetNode<Area2D>("MouseArea");
        SetResourceTexture();
    }

    public void SpawnPlant()
    {
        UpdateGrowthTexture();
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

        Item item = resource_item.Clone();
        // 2. Inventar-Prüfung für normalen und letzten Schlag
        int miningAmount = CalculateMiningAmountInt();
        int durabilityAfterHit = current_durability - miningAmount;
        int bonusAmount = (int)(resource_item.amount * miningAmount);

        item.amount =
            miningAmount
            * (int)Skilltree.instance.GetBonusOfCategory(SkillData.TYPE_CATEGORY.MINING_AMOUNT);

        if (durabilityAfterHit > 0)
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
        ShowHitLabel(miningAmount);
        UpdateToolDurability(miningAmount);

        gpu_particles.Emitting = true;

        DiscoverManager.instance.AddDiscovery(resource_item.info);
        if (current_durability <= 0)
        {
            HandleBreak(bonusAmount);
            return;
        }

        StartTimerBar(TimerBar.STATE.COOLDOWN, click_cooldown_time);
        PlayerUI.AddItemLabelMineableUI(item);
        PlayerInventoryUI.instance.AddItem(item, PlayerInventoryUI.instance.inventory_items);

        hover_menu.InitHoverMenu(this);
    }

    private bool CanReceiveItem(Item item)
    {
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

    private void ShowHitLabel(int miningAmount)
    {
        if (hit_point == null)
            return;

        CharacterBody2D hit_lab = hit_label.Instantiate() as CharacterBody2D;
        hit_lab.GetChild<HitLabel>(0).Init(miningAmount, hit_point);
        IslandManager
            .instance.GetNearestIsland(GetGlobalMousePosition())
            .island_object_save_manager.AddChild(hit_lab);
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
        PlayerUI.AddItemLabelMineableBonusItemUI(resource_item);
        PlayerInventoryUI.instance.AddItem(
            resource_item,
            PlayerInventoryUI.instance.inventory_items
        );

        if (extra_item != null)
        {
            PlayerUI.AddItemLabelMineableBonusItemUI(extra_item);
            PlayerInventoryUI.instance.AddItem(
                extra_item,
                PlayerInventoryUI.instance.inventory_items
            );
        }

        // Collider und Shadow deaktivieren
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
        in_cooldown = true;
        if (state == TimerBar.STATE.SPAWNING)
        {
            if (collision_shape != null)
                collision_shape.Disabled = false;
            timer_bar.InitTimer(max_seconds: time, new_state: state, UpdateGrowthTexture);
            return;
        }

        timer_bar.InitTimer(max_seconds: time, new_state: state);
    }

    //Check if % of Timer is next current frame
    public void UpdateGrowthTexture()
    {
        double progress_percent = timer_bar.GetProgressPercent();
        int frame_index = (int)(progress_percent * (growth_textures.Count - 1));
        timer_bar.UpdateLabel("Growth: " + (int)(progress_percent * 100) + "%");
        if (frame_index < 0)
            frame_index = 0;
        if (frame_index >= growth_textures.Count)
            frame_index = growth_textures.Count - 1;
        sprite_anim_manager.SetTexture2D(growth_textures[frame_index]);
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
            if (ros.last_state != TimerBar.STATE.NONE)
                if (ros.time_left == 0)
                    Reset(from_loading: true);
                else
                    StartTimerBar(ros.last_state, ros.time_left, from_loading: true);
            SetResourceTexture();
        }
        else
            GD.PrintErr("Wrong Resource for MineableObject", resource.ResourceName);
    }

    public void SetResourceTexture()
    {
        if (current_durability >= 0 && mine_textures != null)
            if (mine_textures.Count > current_durability)
                if (mine_textures[current_durability] != null)
                    SetTextureToSpriteManager(mine_textures[current_durability]);
    }
}
